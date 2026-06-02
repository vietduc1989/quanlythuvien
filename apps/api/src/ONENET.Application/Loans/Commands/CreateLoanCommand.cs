using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ONENET.Application.Common.Interfaces;
using ONENET.Domain.Common;
using ONENET.Domain.Entities;
using ONENET.Domain.Enums;

namespace ONENET.Application.Loans.Commands;

public record CreateLoanCommand(Guid ReaderId, List<Guid> BookIds) : IRequest<Result<Guid>>;

public class CreateLoanCommandValidator : AbstractValidator<CreateLoanCommand>
{
    public CreateLoanCommandValidator()
    {
        RuleFor(x => x.ReaderId)
            .NotEmpty().WithMessage("Mã độc giả không được để trống.");

        RuleFor(x => x.BookIds)
            .NotEmpty().WithMessage("Danh sách sách mượn không được để trống.")
            .Must(x => x != null && x.Count >= 1 && x.Count <= 5)
            .WithMessage("Mỗi lần mượn phải chọn từ 1 đến tối đa 5 cuốn sách.");
    }
}

public class CreateLoanCommandHandler : IRequestHandler<CreateLoanCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;

    public CreateLoanCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Guid>> Handle(CreateLoanCommand request, CancellationToken cancellationToken)
    {
        // 1. Kiểm tra độc giả tồn tại
        var reader = await _context.Readers
            .FirstOrDefaultAsync(r => r.Id == request.ReaderId && !r.IsDeleted, cancellationToken);

        if (reader == null)
        {
            return Result<Guid>.Failure("Độc giả không tồn tại trong hệ thống.");
        }

        // 2. Kiểm tra hiệu lực thẻ độc giả
        if (reader.Status != ReaderStatus.Active)
        {
            return Result<Guid>.Failure("Thẻ độc giả đã bị khóa hoặc không hoạt động.");
        }

        if (reader.ExpiryDate < DateTime.UtcNow)
        {
            return Result<Guid>.Failure("Thẻ độc giả đã hết hạn sử dụng.");
        }

        // 3. Kiểm tra số lượng sách đang mượn
        var currentBorrowedCount = await _context.ChiTietPhieuMuons
            .Include(ct => ct.PhieuMuon)
            .Where(ct => ct.PhieuMuon!.DocGiaId == request.ReaderId && 
                         ct.TrangThaiChiTiet == "DangMuon" && 
                         !ct.IsDeleted)
            .CountAsync(cancellationToken);

        if (currentBorrowedCount + request.BookIds.Count > 5)
        {
            return Result<Guid>.Failure($"Độc giả chỉ được mượn tối đa 5 cuốn sách. Hiện độc giả đang mượn {currentBorrowedCount} cuốn và yêu cầu mượn thêm {request.BookIds.Count} cuốn.");
        }

        // 4. Kiểm tra tồn kho cho từng cuốn sách
        var books = await _context.Books
            .Where(b => request.BookIds.Contains(b.Id) && !b.IsDeleted)
            .ToListAsync(cancellationToken);

        if (books.Count != request.BookIds.Distinct().Count())
        {
            return Result<Guid>.Failure("Một hoặc nhiều mã sách yêu cầu không tồn tại.");
        }

        foreach (var book in books)
        {
            if (book.Quantity <= 0)
            {
                return Result<Guid>.Failure($"Sách '{book.Title}' đã hết sách trong kho.");
            }
        }

        // 5. Khởi tạo phiếu mượn
        var ngayMuon = DateTime.UtcNow;
        var ngayHenTra = ngayMuon.AddDays(14);
        var phieuMuon = new PhieuMuon(request.ReaderId, ngayMuon, ngayHenTra);

        // 6. Tạo chi tiết phiếu mượn & giảm tồn kho
        foreach (var book in books)
        {
            var chiTiet = new ChiTietPhieuMuon(phieuMuon.Id, book.Id);
            phieuMuon.ChiTietPhieuMuons.Add(chiTiet);

            // Cập nhật tồn kho sách
            book.UpdateQuantity(book.Quantity - 1);
        }

        _context.PhieuMuons.Add(phieuMuon);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(phieuMuon.Id);
    }
}

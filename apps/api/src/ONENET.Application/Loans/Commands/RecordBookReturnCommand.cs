using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ONENET.Application.Common.Interfaces;
using ONENET.Domain.Common;
using ONENET.Domain.Enums;

namespace ONENET.Application.Loans.Commands;

public record RecordBookReturnResult(Guid LoanDetailId, bool IsOverdue);

public record RecordBookReturnCommand(Guid LoanDetailId) : IRequest<Result<RecordBookReturnResult>>;

public class RecordBookReturnCommandValidator : AbstractValidator<RecordBookReturnCommand>
{
    public RecordBookReturnCommandValidator()
    {
        RuleFor(x => x.LoanDetailId)
            .NotEmpty().WithMessage("Mã chi tiết phiếu mượn không được để trống.");
    }
}

public class RecordBookReturnCommandHandler : IRequestHandler<RecordBookReturnCommand, Result<RecordBookReturnResult>>
{
    private readonly IApplicationDbContext _context;

    public RecordBookReturnCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<RecordBookReturnResult>> Handle(RecordBookReturnCommand request, CancellationToken cancellationToken)
    {
        // 1. Tìm chi tiết phiếu mượn kèm theo phiếu mượn cha
        var chiTiet = await _context.ChiTietPhieuMuons
            .Include(ct => ct.PhieuMuon)
            .FirstOrDefaultAsync(ct => ct.Id == request.LoanDetailId && !ct.IsDeleted, cancellationToken);

        if (chiTiet == null)
        {
            return Result<RecordBookReturnResult>.Failure("Chi tiết phiếu mượn không tồn tại trong hệ thống.");
        }

        if (chiTiet.TrangThaiChiTiet == "DaTra")
        {
            return Result<RecordBookReturnResult>.Failure("Sách này đã được trả trước đó.");
        }

        // 2. Tìm cuốn sách để hoàn trả tồn kho
        var book = await _context.Books
            .FirstOrDefaultAsync(b => b.Id == chiTiet.SachId && !b.IsDeleted, cancellationToken);

        if (book == null)
        {
            return Result<RecordBookReturnResult>.Failure("Sách tương ứng không tồn tại hoặc đã bị xóa.");
        }

        // 3. Thực hiện ghi nhận trả sách
        var ngayTraThucTe = DateTime.UtcNow;
        chiTiet.NgayTraThucTe = ngayTraThucTe;
        chiTiet.TrangThaiChiTiet = "DaTra";

        // Kiểm tra quá hạn
        if (ngayTraThucTe > chiTiet.PhieuMuon!.NgayHenTra)
        {
            chiTiet.IsQuaHan = true;
        }
        else
        {
            chiTiet.IsQuaHan = false;
        }

        // 4. Tăng tồn kho sách +1
        book.UpdateQuantity(book.Quantity + 1);

        // 5. Kiểm tra nếu tất cả sách trong phiếu mượn đã được trả hết
        var otherDetails = await _context.ChiTietPhieuMuons
            .Where(ct => ct.PhieuMuonId == chiTiet.PhieuMuonId && ct.Id != chiTiet.Id && !ct.IsDeleted)
            .ToListAsync(cancellationToken);

        bool allReturned = otherDetails.All(ct => ct.TrangThaiChiTiet == "DaTra");
        if (allReturned)
        {
            chiTiet.PhieuMuon.TrangThaiPhieuMuon = LoanStatus.DaTraHoanTat;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return Result<RecordBookReturnResult>.Success(new RecordBookReturnResult(chiTiet.Id, chiTiet.IsQuaHan));
    }
}

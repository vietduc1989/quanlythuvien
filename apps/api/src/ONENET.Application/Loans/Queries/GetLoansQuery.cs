using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ONENET.Application.Common.Interfaces;
using ONENET.Application.Common.Models;
using ONENET.Application.Loans.DTOs;
using ONENET.Domain.Common;
using ONENET.Domain.Entities;
using ONENET.Domain.Enums;

namespace ONENET.Application.Loans.Queries;

public record GetLoansQuery(
    Guid? ReaderId = null,
    bool? IsOverdue = null,
    int PageNumber = 1,
    int PageSize = 10
) : IRequest<ONENET.Domain.Common.Result<PaginatedList<LoanDto>>>;

public class GetLoansQueryValidator : AbstractValidator<GetLoansQuery>
{
    public GetLoansQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1).WithMessage("Số trang phải lớn hơn hoặc bằng 1.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("Kích thước trang phải từ 1 đến 100.");
    }
}

public class GetLoansQueryHandler : IRequestHandler<GetLoansQuery, ONENET.Domain.Common.Result<PaginatedList<LoanDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetLoansQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ONENET.Domain.Common.Result<PaginatedList<LoanDto>>> Handle(GetLoansQuery request, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;

        var query = _context.PhieuMuons
            .Include(pm => pm.ChiTietPhieuMuons)
            .Where(pm => !pm.IsDeleted);

        // Lọc theo độc giả
        if (request.ReaderId.HasValue)
        {
            query = query.Where(pm => pm.DocGiaId == request.ReaderId.Value);
        }

        // Lọc theo quá hạn
        if (request.IsOverdue.HasValue)
        {
            if (request.IsOverdue.Value)
            {
                query = query.Where(pm => pm.ChiTietPhieuMuons.Any(ct => 
                    !ct.IsDeleted && 
                    (ct.IsQuaHan || (ct.TrangThaiChiTiet == "DangMuon" && pm.NgayHenTra < now))
                ));
            }
            else
            {
                query = query.Where(pm => !pm.ChiTietPhieuMuons.Any(ct => 
                    !ct.IsDeleted && 
                    (ct.IsQuaHan || (ct.TrangThaiChiTiet == "DangMuon" && pm.NgayHenTra < now))
                ));
            }
        }

        // Sắp xếp giảm dần theo ngày mượn mới nhất
        query = query.OrderByDescending(pm => pm.NgayMuon);

        // Tạo danh sách phân trang PhieuMuon
        var count = await query.CountAsync(cancellationToken);
        var items = await query.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToListAsync(cancellationToken);

        // Lấy tên độc giả liên quan
        var readerIds = items.Select(pm => pm.DocGiaId).Distinct().ToList();
        var readers = await _context.Readers
            .Where(r => readerIds.Contains(r.Id))
            .ToDictionaryAsync(r => r.Id, r => r.FullName, cancellationToken);

        // Lấy tên sách liên quan
        var bookIds = items.SelectMany(pm => pm.ChiTietPhieuMuons).Select(ct => ct.SachId).Distinct().ToList();
        var books = await _context.Books
            .Where(b => bookIds.Contains(b.Id))
            .ToDictionaryAsync(b => b.Id, b => b.Title, cancellationToken);

        // Ánh xạ sang danh sách LoanDto
        var loanDtos = items.Select(pm => new LoanDto
        {
            LoanId = pm.Id,
            ReaderId = pm.DocGiaId,
            ReaderName = readers.TryGetValue(pm.DocGiaId, out var readerName) ? readerName : "Độc giả không rõ",
            LoanDate = pm.NgayMuon,
            DueDate = pm.NgayHenTra,
            LoanStatus = pm.TrangThaiPhieuMuon.ToString(),
            Details = pm.ChiTietPhieuMuons.Where(ct => !ct.IsDeleted).Select(ct => new LoanDetailDto
            {
                LoanDetailId = ct.Id,
                BookId = ct.SachId,
                BookTitle = books.TryGetValue(ct.SachId, out var title) ? title : "Sách không rõ",
                ActualReturnDate = ct.NgayTraThucTe,
                DetailStatus = ct.TrangThaiChiTiet.ToString(),
                IsOverdue = ct.IsQuaHan || (ct.TrangThaiChiTiet == "DangMuon" && pm.NgayHenTra < now)
            }).ToList()
        }).ToList();

        // Tạo đối tượng PaginatedList<LoanDto> mới
        var result = new PaginatedList<LoanDto>(loanDtos, count, request.PageNumber, request.PageSize);

        return ONENET.Domain.Common.Result<PaginatedList<LoanDto>>.Success(result);
    }
}

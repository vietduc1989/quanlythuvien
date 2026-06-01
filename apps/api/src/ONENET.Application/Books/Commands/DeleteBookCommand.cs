using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ONENET.Application.Common.Interfaces;
using ONENET.Domain.Common;

namespace ONENET.Application.Books.Commands;

public record DeleteBookCommand(Guid Id) : IRequest<Result>;

public class DeleteBookCommandHandler : IRequestHandler<DeleteBookCommand, Result>
{
    private readonly IAppDbContext _context;

    public DeleteBookCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(DeleteBookCommand request, CancellationToken cancellationToken)
    {
        var book = await _context.Books
            .FirstOrDefaultAsync(b => b.Id == request.Id && !b.IsDeleted, cancellationToken);

        if (book == null)
        {
            return Result.Failure($"Không tìm thấy sách với ID '{request.Id}'.");
        }

        // Business Rule: Không cho phép xóa sách nếu đang có độc giả mượn
        // (Đây là stub, sẽ mở rộng để kiểm tra bảng Loans/Borrows khi các tính năng kia được cài đặt)
        bool isCurrentlyBorrowed = false; 

        if (isCurrentlyBorrowed)
        {
            return Result.Failure("Không thể xóa sách do đang có độc giả mượn.");
        }

        // Soft delete
        book.IsDeleted = true;
        book.LastModifiedAt = DateTime.UtcNow;
        book.LastModifiedBy = "System";

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

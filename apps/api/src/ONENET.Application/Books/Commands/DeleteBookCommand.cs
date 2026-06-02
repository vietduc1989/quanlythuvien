using MediatR;
using FluentValidation;
using ONENET.Domain.Entities;
using ONENET.Application.Common.Interfaces;
using ONENET.Application.Common.Exceptions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System;

namespace ONENET.Application.Books.Commands;

public record DeleteBookCommand(Guid Id) : IRequest;

public class DeleteBookCommandValidator : AbstractValidator<DeleteBookCommand>
{
    public DeleteBookCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("ID sách không được để trống.");
    }
}

public class DeleteBookCommandHandler : IRequestHandler<DeleteBookCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteBookCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeleteBookCommand request, CancellationToken cancellationToken)
    {
        var book = await _context.Books
            .FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

        if (book == null)
        {
            throw new NotFoundException(nameof(Book), request.Id);
        }

        bool isBorrowed = await _context.Set<ChiTietPhieuMuon>()
            .AnyAsync(ld => ld.SachId == request.Id && ld.TrangThaiChiTiet == "DangMuon", cancellationToken);

        if (isBorrowed)
        {
            throw new FluentValidation.ValidationException("Không thể xóa sách vì hiện tại đang có độc giả mượn cuốn sách này.");
        }

        // Soft Delete
        book.IsDeleted = true;
        book.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
    }
}

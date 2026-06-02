using MediatR;
using FluentValidation;
using ONENET.Domain.Entities;
using ONENET.Domain.Enums;
using ONENET.Application.Common.Interfaces;
using ONENET.Application.Common.Exceptions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System;

namespace ONENET.Application.Books.Commands;

public record UpdateBookCommand(
    Guid Id,
    string Title,
    string Author,
    string Category,
    string Publisher,
    int PublishYear,
    int Quantity,
    string? ShelfLocation
) : IRequest;

public class UpdateBookCommandValidator : AbstractValidator<UpdateBookCommand>
{
    public UpdateBookCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("ID sách không được để trống.");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Tên sách không được để trống.")
            .MaximumLength(200).WithMessage("Tên sách không được vượt quá 200 ký tự.");

        RuleFor(x => x.Author)
            .NotEmpty().WithMessage("Tác giả không được để trống.")
            .MaximumLength(100).WithMessage("Tác giả không được vượt quá 100 ký tự.");

        RuleFor(x => x.Category)
            .NotEmpty().WithMessage("Thể loại không được để trống.")
            .MaximumLength(100).WithMessage("Thể loại không được vượt quá 100 ký tự.");

        RuleFor(x => x.Publisher)
            .NotEmpty().WithMessage("Nhà xuất bản không được để trống.")
            .MaximumLength(100).WithMessage("Nhà xuất bản không được vượt quá 100 ký tự.");

        RuleFor(x => x.PublishYear)
            .Must(year => year <= DateTime.Today.Year).WithMessage("Năm xuất bản không hợp lệ.");

        RuleFor(x => x.Quantity)
            .GreaterThanOrEqualTo(0).WithMessage("Số lượng tồn kho không được nhỏ hơn 0.");
    }
}

public class UpdateBookCommandHandler : IRequestHandler<UpdateBookCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateBookCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdateBookCommand request, CancellationToken cancellationToken)
    {
        var book = await _context.Books
            .FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

        if (book == null)
        {
            throw new NotFoundException(nameof(Book), request.Id);
        }

        // Update properties
        book.Title = request.Title.Trim();
        book.Author = request.Author.Trim();
        book.Category = request.Category.Trim();
        book.Publisher = request.Publisher.Trim();
        book.PublishYear = request.PublishYear;
        book.ShelfLocation = request.ShelfLocation?.Trim();

        // Update quantity & automatically adjust status (OutOfStock if Quantity = 0 else Available)
        book.UpdateQuantity(request.Quantity);

        book.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
    }
}

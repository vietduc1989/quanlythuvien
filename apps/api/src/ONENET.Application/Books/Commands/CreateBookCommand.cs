using MediatR;
using FluentValidation;
using ONENET.Domain.Entities;
using ONENET.Domain.Enums;
using ONENET.Application.Common.Interfaces;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System;

namespace ONENET.Application.Books.Commands;

public record CreateBookCommand(
    string? BookCode,
    string Title,
    string Author,
    string Category,
    string Publisher,
    int PublishYear,
    int Quantity,
    string? ShelfLocation
) : IRequest<Guid>;

public class CreateBookCommandValidator : AbstractValidator<CreateBookCommand>
{
    public CreateBookCommandValidator()
    {
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

public class CreateBookCommandHandler : IRequestHandler<CreateBookCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateBookCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateBookCommand request, CancellationToken cancellationToken)
    {
        // 1. Generate or validate BookCode
        string code = string.IsNullOrWhiteSpace(request.BookCode)
            ? $"BK-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}"
            : request.BookCode.Trim();

        // 2. Check uniqueness of BookCode
        bool exists = await _context.Books.AnyAsync(b => b.BookCode == code, cancellationToken);
        if (exists)
        {
            throw new ValidationException($"Mã sách '{code}' đã tồn tại trong hệ thống.");
        }

        // 3. Create Book entity
        var book = new Book
        {
            BookCode = code,
            Title = request.Title.Trim(),
            Author = request.Author.Trim(),
            Category = request.Category.Trim(),
            Publisher = request.Publisher.Trim(),
            PublishYear = request.PublishYear,
            ShelfLocation = request.ShelfLocation?.Trim()
        };

        // 4. Update quantity & automatically set initial status
        book.UpdateQuantity(request.Quantity);

        // 5. Save to database
        _context.Books.Add(book);
        await _context.SaveChangesAsync(cancellationToken);

        return book.Id;
    }
}

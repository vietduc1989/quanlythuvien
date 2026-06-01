using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ONENET.Application.Common.Interfaces;
using ONENET.Domain.Common;
using ONENET.Domain.Entities;

namespace ONENET.Application.Books.Commands;

public record CreateBookCommand : IRequest<Result<Guid>>
{
    public string BookCode { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string Author { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public string Publisher { get; init; } = string.Empty;
    public int PublishYear { get; init; }
    public int Quantity { get; init; }
    public string? ShelfLocation { get; init; }
}

public class CreateBookCommandValidator : AbstractValidator<CreateBookCommand>
{
    public CreateBookCommandValidator()
    {
        RuleFor(x => x.BookCode)
            .NotEmpty().WithMessage("Mã sách không được để trống.")
            .MaximumLength(50).WithMessage("Mã sách không quá 50 ký tự.");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Tên sách không được để trống.")
            .MaximumLength(200).WithMessage("Tên sách không quá 200 ký tự.");

        RuleFor(x => x.Author)
            .NotEmpty().WithMessage("Tác giả không được để trống.")
            .MaximumLength(100).WithMessage("Tác giả không quá 100 ký tự.");

        RuleFor(x => x.Category)
            .NotEmpty().WithMessage("Thể loại không được để trống.")
            .MaximumLength(100).WithMessage("Thể loại không quá 100 ký tự.");

        RuleFor(x => x.Publisher)
            .NotEmpty().WithMessage("Nhà xuất bản không được để trống.")
            .MaximumLength(100).WithMessage("Nhà xuất bản không quá 100 ký tự.");

        RuleFor(x => x.PublishYear)
            .Must(year => year <= DateTime.UtcNow.Year).WithMessage("Năm xuất bản không hợp lệ.");

        RuleFor(x => x.Quantity)
            .GreaterThanOrEqualTo(0).WithMessage("Số lượng không được nhỏ hơn 0.");

        RuleFor(x => x.ShelfLocation)
            .MaximumLength(50).WithMessage("Vị trí kệ không quá 50 ký tự.");
    }
}

public class CreateBookCommandHandler : IRequestHandler<CreateBookCommand, Result<Guid>>
{
    private readonly IAppDbContext _context;

    public CreateBookCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Guid>> Handle(CreateBookCommand request, CancellationToken cancellationToken)
    {
        // Check for unique BookCode
        var isBookCodeExists = await _context.Books
            .AnyAsync(b => b.BookCode == request.BookCode && !b.IsDeleted, cancellationToken);

        if (isBookCodeExists)
        {
            return Result.Failure<Guid>($"Mã sách '{request.BookCode}' đã tồn tại trong hệ thống.");
        }

        var book = new Book(
            request.BookCode,
            request.Title,
            request.Author,
            request.Category,
            request.Publisher,
            request.PublishYear,
            request.Quantity,
            request.ShelfLocation
        );

        _context.Books.Add(book);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success(book.Id);
    }
}

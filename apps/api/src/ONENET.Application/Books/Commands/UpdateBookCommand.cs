using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ONENET.Application.Common.Interfaces;
using ONENET.Domain.Common;

namespace ONENET.Application.Books.Commands;

public record UpdateBookCommand : IRequest<Result>
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Author { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public string Publisher { get; init; } = string.Empty;
    public int PublishYear { get; init; }
    public int Quantity { get; init; }
    public string? ShelfLocation { get; init; }
}

public class UpdateBookCommandValidator : AbstractValidator<UpdateBookCommand>
{
    public UpdateBookCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("ID sách không hợp lệ.");

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

public class UpdateBookCommandHandler : IRequestHandler<UpdateBookCommand, Result>
{
    private readonly IAppDbContext _context;

    public UpdateBookCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(UpdateBookCommand request, CancellationToken cancellationToken)
    {
        var book = await _context.Books
            .FirstOrDefaultAsync(b => b.Id == request.Id && !b.IsDeleted, cancellationToken);

        if (book == null)
        {
            return Result.Failure($"Không tìm thấy sách với ID '{request.Id}'.");
        }

        book.Title = request.Title;
        book.Author = request.Author;
        book.Category = request.Category;
        book.Publisher = request.Publisher;
        book.PublishYear = request.PublishYear;
        book.ShelfLocation = request.ShelfLocation;
        
        book.UpdateStock(request.Quantity);

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

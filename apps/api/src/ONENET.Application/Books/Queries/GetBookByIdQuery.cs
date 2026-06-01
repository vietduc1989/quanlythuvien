using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ONENET.Application.Books.DTOs;
using ONENET.Application.Common.Interfaces;
using ONENET.Domain.Common;

namespace ONENET.Application.Books.Queries;

public record GetBookByIdQuery(Guid Id) : IRequest<Result<BookDto>>;

public class GetBookByIdQueryHandler : IRequestHandler<GetBookByIdQuery, Result<BookDto>>
{
    private readonly IAppDbContext _context;

    public GetBookByIdQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<BookDto>> Handle(GetBookByIdQuery request, CancellationToken cancellationToken)
    {
        var book = await _context.Books
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == request.Id && !b.IsDeleted, cancellationToken);

        if (book == null)
        {
            return Result.Failure<BookDto>($"Không tìm thấy chi tiết sách với ID '{request.Id}'.");
        }

        var dto = new BookDto
        {
            Id = book.Id,
            BookCode = book.BookCode,
            Title = book.Title,
            Author = book.Author,
            Category = book.Category,
            Publisher = book.Publisher,
            PublishYear = book.PublishYear,
            Quantity = book.Quantity,
            ShelfLocation = book.ShelfLocation,
            Status = book.Status.ToString()
        };

        return Result.Success(dto);
    }
}

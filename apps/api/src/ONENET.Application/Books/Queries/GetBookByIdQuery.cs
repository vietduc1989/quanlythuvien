using MediatR;
using ONENET.Application.Common.Interfaces;
using ONENET.Application.Common.Exceptions;
using ONENET.Domain.Enums;
using ONENET.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System;

namespace ONENET.Application.Books.Queries;

public record GetBookByIdQuery(Guid Id) : IRequest<BookDto>;

public class GetBookByIdQueryHandler : IRequestHandler<GetBookByIdQuery, BookDto>
{
    private readonly IApplicationDbContext _context;

    public GetBookByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<BookDto> Handle(GetBookByIdQuery request, CancellationToken cancellationToken)
    {
        var book = await _context.Books
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

        if (book == null)
        {
            throw new NotFoundException(nameof(Book), request.Id);
        }

        return new BookDto(
            book.Id,
            book.BookCode,
            book.Title,
            book.Author,
            book.Category,
            book.Publisher,
            book.PublishYear,
            book.Quantity,
            book.ShelfLocation,
            book.Status == BookStatus.Available ? "Available" : "OutOfStock"
        );
    }
}

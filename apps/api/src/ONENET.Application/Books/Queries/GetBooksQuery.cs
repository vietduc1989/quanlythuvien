using MediatR;
using ONENET.Domain.Entities;
using ONENET.Domain.Enums;
using ONENET.Application.Common.Interfaces;
using ONENET.Application.Common.Models;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System;

namespace ONENET.Application.Books.Queries;

public record GetBooksQuery(
    string? Search,
    string? Category,
    BookStatus? Status,
    int PageNumber = 1,
    int PageSize = 10
) : IRequest<PaginatedList<BookDto>>;

public record BookDto(
    Guid Id,
    string BookCode,
    string Title,
    string Author,
    string Category,
    string Publisher,
    int PublishYear,
    int Quantity,
    string? ShelfLocation,
    string Status
);

public class GetBooksQueryHandler : IRequestHandler<GetBooksQuery, PaginatedList<BookDto>>
{
    private readonly IApplicationDbContext _context;

    public GetBooksQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedList<BookDto>> Handle(GetBooksQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Books.AsNoTracking();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var searchPattern = $"%{request.Search.Trim().ToLower()}%";
            query = query.Where(b => EF.Functions.Like(b.Title.ToLower(), searchPattern) 
                                  || EF.Functions.Like(b.Author.ToLower(), searchPattern));
        }

        if (!string.IsNullOrWhiteSpace(request.Category))
        {
            query = query.Where(b => b.Category.ToLower() == request.Category.Trim().ToLower());
        }

        if (request.Status.HasValue)
        {
            query = query.Where(b => b.Status == request.Status.Value);
        }

        int count = await query.CountAsync(cancellationToken);
        
        int pageNumber = request.PageNumber <= 0 ? 1 : request.PageNumber;
        int pageSize = request.PageSize <= 0 ? 10 : request.PageSize;

        var items = await query
            .OrderBy(b => b.Title)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(b => new BookDto(
                b.Id,
                b.BookCode,
                b.Title,
                b.Author,
                b.Category,
                b.Publisher,
                b.PublishYear,
                b.Quantity,
                b.ShelfLocation,
                b.Status == BookStatus.Available ? "Available" : "OutOfStock"
            ))
            .ToListAsync(cancellationToken);

        return new PaginatedList<BookDto>(items, count, pageNumber, pageSize);
    }
}

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ONENET.Application.Books.DTOs;
using ONENET.Application.Common.Interfaces;
using ONENET.Application.Common.Models;
using ONENET.Domain.Common;

namespace ONENET.Application.Books.Queries;

public record GetBooksQuery : IRequest<Result<PaginatedList<BookDto>>>
{
    public string? Search { get; init; }
    public string? Category { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public class GetBooksQueryHandler : IRequestHandler<GetBooksQuery, Result<PaginatedList<BookDto>>>
{
    private readonly IAppDbContext _context;

    public GetBooksQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PaginatedList<BookDto>>> Handle(GetBooksQuery request, CancellationToken cancellationToken)
    {
        var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
        var pageSize = request.PageSize < 1 ? 10 : (request.PageSize > 100 ? 100 : request.PageSize);

        var query = _context.Books
            .AsNoTracking()
            .Where(b => !b.IsDeleted);

        // Filter by Search (Title or Author keyword)
        if (!string.IsNullOrEmpty(request.Search))
        {
            var searchLower = request.Search.ToLower();
            query = query.Where(b => b.Title.ToLower().Contains(searchLower) || b.Author.ToLower().Contains(searchLower));
        }

        // Filter by Category
        if (!string.IsNullOrEmpty(request.Category))
        {
            var categoryLower = request.Category.ToLower();
            query = query.Where(b => b.Category.ToLower() == categoryLower);
        }

        // Project to DTO
        var dtoQuery = query.Select(b => new BookDto
        {
            Id = b.Id,
            BookCode = b.BookCode,
            Title = b.Title,
            Author = b.Author,
            Category = b.Category,
            Publisher = b.Publisher,
            PublishYear = b.PublishYear,
            Quantity = b.Quantity,
            ShelfLocation = b.ShelfLocation,
            Status = b.Status.ToString()
        });

        var paginatedResult = await PaginatedList<BookDto>.CreateAsync(dtoQuery, pageNumber, pageSize);

        return Result.Success(paginatedResult);
    }
}

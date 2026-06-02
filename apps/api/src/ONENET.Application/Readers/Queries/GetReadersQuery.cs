using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ONENET.Application.Common.Interfaces;
using ONENET.Application.Common.Models;
using ONENET.Application.Readers.DTOs;
using ONENET.Domain.Common;
using ONENET.Domain.Enums;

namespace ONENET.Application.Readers.Queries;

public record GetReadersQuery : IRequest<Result<PaginatedList<ReaderDto>>>
{
    public string? Search { get; init; }
    public ReaderStatus? Status { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public class GetReadersQueryHandler : IRequestHandler<GetReadersQuery, Result<PaginatedList<ReaderDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetReadersQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PaginatedList<ReaderDto>>> Handle(GetReadersQuery request, CancellationToken cancellationToken)
    {
        var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
        var pageSize = request.PageSize < 1 ? 10 : (request.PageSize > 100 ? 100 : request.PageSize);

        var query = _context.Readers
            .AsNoTracking()
            .Where(r => !r.IsDeleted);

        // Filter by Search (FullName, ReaderCode, PhoneNumber, Email)
        if (!string.IsNullOrEmpty(request.Search))
        {
            var searchLower = request.Search.ToLower();
            query = query.Where(r => 
                r.FullName.ToLower().Contains(searchLower) || 
                r.ReaderCode.ToLower().Contains(searchLower) || 
                r.PhoneNumber.Contains(request.Search) || 
                (r.Email != null && r.Email.ToLower().Contains(searchLower))
            );
        }

        // Filter by Status
        if (request.Status.HasValue)
        {
            query = query.Where(r => r.Status == request.Status.Value);
        }

        // Project to DTO
        var dtoQuery = query.Select(r => new ReaderDto
        {
            Id = r.Id,
            ReaderCode = r.ReaderCode,
            FullName = r.FullName,
            DateOfBirth = r.DateOfBirth,
            PhoneNumber = r.PhoneNumber,
            Email = r.Email,
            Address = r.Address,
            RegistrationDate = r.RegistrationDate,
            ExpiryDate = r.ExpiryDate,
            Status = r.Status.ToString()
        });

        var paginatedResult = await PaginatedList<ReaderDto>.CreateAsync(dtoQuery, pageNumber, pageSize);

        return Result.Success(paginatedResult);
    }
}

// QUAN-20260601-105011
using MediatR;
using ONENET.Application.Common.Models;
using ONENET.Domain.Common;
using ONENET.Application.Features.Readers.DTOs;
using ONENET.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace ONENET.Application.Features.Readers.Queries.GetReadersList;

/// <summary>
/// Handler xử lý việc lấy danh sách độc giả.
/// </summary>
public class GetReadersListQueryHandler : IRequestHandler<GetReadersListQuery, ONENET.Domain.Common.Result<PaginatedList<ReaderListItemDto>>>
{
    private readonly IReaderRepository _readerRepository;
    private readonly ILogger<GetReadersListQueryHandler> _logger;

    public GetReadersListQueryHandler(IReaderRepository readerRepository, ILogger<GetReadersListQueryHandler> logger)
    {
        _readerRepository = readerRepository;
        _logger = logger;
    }

    public async Task<ONENET.Domain.Common.Result<PaginatedList<ReaderListItemDto>>> Handle(GetReadersListQuery request, CancellationToken cancellationToken)
    {
        var readers = await _readerRepository.GetPagedListAsync(
            request.SearchTerm,
            request.Status,
            request.PageIndex,
            request.PageSize,
            request.SortBy,
            request.SortOrder,
            cancellationToken);

        var totalCount = await _readerRepository.CountAsync(request.SearchTerm, request.Status, cancellationToken);

        var readerListItems = readers.Select(r => new ReaderListItemDto
        {
            ReaderId = r.Id,
            ReaderCode = r.ReaderCode,
            FullName = r.FullName,
            PhoneNumber = r.PhoneNumber,
            Email = r.Email,
            Status = r.Status,
            RegistrationDate = DateOnly.FromDateTime(r.RegistrationDate),
            ExpiryDate = DateOnly.FromDateTime(r.ExpiryDate)
        }).ToList();

        var paginatedList = new PaginatedList<ReaderListItemDto>(
            readerListItems,
            totalCount,
            request.PageIndex,
            request.PageSize);

        _logger.LogInformation("Retrieved {Count} readers for page {PageIndex} with search term '{SearchTerm}' and status '{Status}'.",
            readerListItems.Count, request.PageIndex, request.SearchTerm, request.Status);

        return ONENET.Domain.Common.Result<PaginatedList<ReaderListItemDto>>.Success(paginatedList);
    }
}
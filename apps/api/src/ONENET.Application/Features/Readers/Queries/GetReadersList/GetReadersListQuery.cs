// QUAN-20260601-105011
using MediatR;
using ONENET.Application.Common.Models;
using ONENET.Domain.Common;
using ONENET.Application.Features.Readers.DTOs;
using ONENET.Domain.Enums;

namespace ONENET.Application.Features.Readers.Queries.GetReadersList;

/// <summary>
/// Query để lấy danh sách độc giả với phân trang, tìm kiếm và lọc.
/// </summary>
public record GetReadersListQuery : IRequest<ONENET.Domain.Common.Result<PaginatedList<ReaderListItemDto>>>
{
    public string? SearchTerm { get; init; }
    public ReaderStatus? Status { get; init; }
    public int PageIndex { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string SortBy { get; init; } = "FullName";
    public string SortOrder { get; init; } = "Asc"; // "Asc" or "Desc"
}
// QUAN-20260601-105011
using MediatR;
using ONENET.Application.Common.Models;
using ONENET.Application.Features.Readers.DTOs;

namespace ONENET.Application.Features.Readers.Queries.GetReaderById;

/// <summary>
/// Query để lấy thông tin chi tiết độc giả theo ID.
/// </summary>
public record GetReaderByIdQuery(Guid Id) : IRequest<Result<ReaderDto>>;
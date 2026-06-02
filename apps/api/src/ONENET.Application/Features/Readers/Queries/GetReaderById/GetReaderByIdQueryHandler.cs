// QUAN-20260601-105011
using MediatR;
using ONENET.Application.Common.Exceptions;
using ONENET.Domain.Common;
using ONENET.Application.Features.Readers.DTOs;
using ONENET.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace ONENET.Application.Features.Readers.Queries.GetReaderById;

/// <summary>
/// Handler xử lý việc lấy thông tin chi tiết độc giả theo ID.
/// </summary>
public class GetReaderByIdQueryHandler : IRequestHandler<GetReaderByIdQuery, Result<ReaderDto>>
{
    private readonly IReaderRepository _readerRepository;
    private readonly ILogger<GetReaderByIdQueryHandler> _logger;

    public GetReaderByIdQueryHandler(IReaderRepository readerRepository, ILogger<GetReaderByIdQueryHandler> logger)
    {
        _readerRepository = readerRepository;
        _logger = logger;
    }

    public async Task<Result<ReaderDto>> Handle(GetReaderByIdQuery request, CancellationToken cancellationToken)
    {
        var reader = await _readerRepository.GetByIdAsync(request.Id, cancellationToken);

        if (reader == null)
        {
            _logger.LogWarning("GetReaderByIdQuery failed: Reader with ID {ReaderId} not found.", request.Id);
            throw new NotFoundException(nameof(reader), request.Id);
        }

        var readerDto = new ReaderDto
        {
            ReaderId = reader.Id,
            ReaderCode = reader.ReaderCode,
            FullName = reader.FullName,
            DateOfBirth = DateOnly.FromDateTime(reader.DateOfBirth),
            PhoneNumber = reader.PhoneNumber,
            Email = reader.Email,
            Address = reader.Address,
            RegistrationDate = DateOnly.FromDateTime(reader.RegistrationDate),
            ExpiryDate = DateOnly.FromDateTime(reader.ExpiryDate),
            Status = reader.Status
        };

        _logger.LogInformation("Reader with ID {ReaderId} retrieved successfully.", request.Id);
        return Result<ReaderDto>.Success(readerDto);
    }
}
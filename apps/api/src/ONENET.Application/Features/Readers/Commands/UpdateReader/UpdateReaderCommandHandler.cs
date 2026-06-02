// QUAN-20260601-105011
using MediatR;
using ONENET.Application.Common.Exceptions;
using ONENET.Application.Common.Interfaces;
using ONENET.Domain.Common;
using ONENET.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace ONENET.Application.Features.Readers.Commands.UpdateReader;

/// <summary>
/// Handler xử lý việc cập nhật thông tin độc giả.
/// </summary>
public class UpdateReaderCommandHandler : IRequestHandler<UpdateReaderCommand, Result<Guid>>
{
    private readonly IReaderRepository _readerRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;
    private readonly ILogger<UpdateReaderCommandHandler> _logger;

    public UpdateReaderCommandHandler(
        IReaderRepository readerRepository,
        IUnitOfWork unitOfWork,
        ICurrentUser currentUser,
        ILogger<UpdateReaderCommandHandler> logger)
    {
        _readerRepository = readerRepository;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(UpdateReaderCommand request, CancellationToken cancellationToken)
    {
        var reader = await _readerRepository.GetByIdAsync(request.ReaderId, cancellationToken);

        if (reader == null)
        {
            _logger.LogWarning("UpdateReaderCommand failed: Reader with ID {ReaderId} not found.", request.ReaderId);
            throw new NotFoundException(nameof(reader), request.ReaderId);
        }

        // Concurrency check: BRD đề cập đến xmin nhưng cũng nói có thể dùng LastModifiedAt.
        // Đây là cách đơn giản để thực hiện optimistic concurrency control.
        if (request.LastModifiedAtClient.HasValue && reader.UpdatedAt.HasValue &&
            reader.UpdatedAt.Value.Ticks != request.LastModifiedAtClient.Value.Ticks)
        {
            // Có xung đột, trả về lỗi 409 Conflict thông qua GlobalExceptionMiddleware
            // Message này sẽ được middleware bắt và chuyển thành lỗi phù hợp
            _logger.LogWarning("UpdateReaderCommand concurrency conflict for Reader ID {ReaderId}. Client's LastModifiedAt: {ClientTime}, DB's LastModifiedAt: {DbTime}",
                request.ReaderId, request.LastModifiedAtClient.Value, reader.UpdatedAt.Value);
            return Result<Guid>.Failure("Concurrency conflict: The reader record has been modified by another user. Please refresh and try again.");
        }

        // BR02: Kiểm tra số điện thoại duy nhất nếu có sự thay đổi
        if (!string.IsNullOrWhiteSpace(request.PhoneNumber) && reader.PhoneNumber != request.PhoneNumber)
        {
            if (await _readerRepository.IsPhoneNumberUniqueAsync(request.PhoneNumber, request.ReaderId, cancellationToken))
            {
                _logger.LogWarning("UpdateReaderCommand failed: Phone number '{PhoneNumber}' already exists for another reader (Reader ID {ReaderId}).",
                    request.PhoneNumber, request.ReaderId);
                return Result<Guid>.Failure($"Phone number '{request.PhoneNumber}' already exists for another reader.");
            }
        }

        // BR05: Đọc giả phải trên 16 tuổi (tính đến ngày đăng ký thẻ)
        var targetDob = request.DateOfBirth ?? DateOnly.FromDateTime(reader.DateOfBirth);
        var targetRegDate = request.RegistrationDate ?? DateOnly.FromDateTime(reader.RegistrationDate);
        var age = targetRegDate.Year - targetDob.Year;
        if (targetDob.AddYears(16) > targetRegDate)
        {
            age--;
        }
        if (age < 16)
        {
            _logger.LogWarning("UpdateReaderCommand failed: Reader with ID {ReaderId} must be at least 16 years old.", request.ReaderId);
            return Result<Guid>.Failure("Độc giả phải từ 16 tuổi trở lên tính đến ngày đăng ký thẻ.");
        }

        reader.Update(
            fullName: request.FullName,
            dateOfBirth: request.DateOfBirth?.ToDateTime(TimeOnly.MinValue),
            phoneNumber: request.PhoneNumber,
            email: request.Email,
            address: request.Address,
            registrationDate: request.RegistrationDate?.ToDateTime(TimeOnly.MinValue),
            expiryDate: request.ExpiryDate?.ToDateTime(TimeOnly.MinValue),
            status: request.Status,
            updatedBy: _currentUser.UserId ?? "System"
        );

        _readerRepository.Update(reader);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Reader with ID {ReaderId} updated successfully by user {UserId}.",
            reader.Id, _currentUser.UserId);

        return Result<Guid>.Success(reader.Id);
    }
}
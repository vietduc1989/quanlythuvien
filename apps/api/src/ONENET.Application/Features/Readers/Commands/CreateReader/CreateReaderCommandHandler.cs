// QUAN-20260601-105011
using MediatR;
using ONENET.Application.Common.Interfaces;
using ONENET.Domain.Common;
using ONENET.Domain.Entities;
using ONENET.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace ONENET.Application.Features.Readers.Commands.CreateReader;

/// <summary>
/// Handler xử lý việc tạo mới độc giả.
/// </summary>
public class CreateReaderCommandHandler : IRequestHandler<CreateReaderCommand, Result<CreateReaderResponse>>
{
    private readonly IReaderRepository _readerRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;
    private readonly IReaderCodeGenerator _readerCodeGenerator;
    private readonly IDateTime _dateTime;
    private readonly ILogger<CreateReaderCommandHandler> _logger;

    public CreateReaderCommandHandler(
        IReaderRepository readerRepository,
        IUnitOfWork unitOfWork,
        ICurrentUser currentUser,
        IReaderCodeGenerator readerCodeGenerator,
        IDateTime dateTime,
        ILogger<CreateReaderCommandHandler> logger)
    {
        _readerRepository = readerRepository;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _readerCodeGenerator = readerCodeGenerator;
        _dateTime = dateTime;
        _logger = logger;
    }

    public async Task<Result<CreateReaderResponse>> Handle(CreateReaderCommand request, CancellationToken cancellationToken)
    {
        // BR02: Kiểm tra số điện thoại duy nhất
        if (await _readerRepository.IsPhoneNumberUniqueAsync(request.PhoneNumber, null, cancellationToken))
        {
            return Result<CreateReaderResponse>.Failure($"Phone number '{request.PhoneNumber}' already exists for another reader.");
        }

        // BR05: Đọc giả phải trên 16 tuổi (tính đến ngày đăng ký thẻ)
        var age = request.RegistrationDate.Year - request.DateOfBirth.Year;
        if (request.DateOfBirth.AddYears(16) > request.RegistrationDate)
        {
            age--;
        }
        if (age < 16)
        {
            _logger.LogWarning("CreateReaderCommand failed: Reader must be at least 16 years old.");
            return Result<CreateReaderResponse>.Failure("Độc giả phải từ 16 tuổi trở lên tính đến ngày đăng ký thẻ.");
        }

        // BR01: Tạo mã độc giả duy nhất
        var readerCode = await _readerCodeGenerator.GenerateUniqueCodeAsync(cancellationToken);

        var reader = Reader.Create(
            readerCode: readerCode,
            fullName: request.FullName,
            dateOfBirth: DateTime.SpecifyKind(request.DateOfBirth.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc),
            phoneNumber: request.PhoneNumber,
            registrationDate: DateTime.SpecifyKind(request.RegistrationDate.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc),
            expiryDate: DateTime.SpecifyKind(request.ExpiryDate.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc),
            email: request.Email,
            address: request.Address,
            status: request.Status,
            createdBy: _currentUser.UserId ?? "System" // Nếu không có người dùng đăng nhập, mặc định là System
        );

        await _readerRepository.AddAsync(reader, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<CreateReaderResponse>.Success(new CreateReaderResponse(reader.Id, reader.ReaderCode));
    }
}
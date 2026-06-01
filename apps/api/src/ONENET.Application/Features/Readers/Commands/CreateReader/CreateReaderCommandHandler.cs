// QUAN-20260601-105011
using MediatR;
using ONENET.Application.Common.Interfaces;
using ONENET.Application.Common.Models;
using ONENET.Domain.Entities;
using ONENET.Domain.Interfaces;

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

    public CreateReaderCommandHandler(
        IReaderRepository readerRepository,
        IUnitOfWork unitOfWork,
        ICurrentUser currentUser,
        IReaderCodeGenerator readerCodeGenerator,
        IDateTime dateTime)
    {
        _readerRepository = readerRepository;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _readerCodeGenerator = readerCodeGenerator;
        _dateTime = dateTime;
    }

    public async Task<Result<CreateReaderResponse>> Handle(CreateReaderCommand request, CancellationToken cancellationToken)
    {
        // BR02: Kiểm tra số điện thoại duy nhất
        if (await _readerRepository.IsPhoneNumberUniqueAsync(request.PhoneNumber, null, cancellationToken))
        {
            return Result<CreateReaderResponse>.Failure($"Phone number '{request.PhoneNumber}' already exists for another reader.");
        }

        // BR01: Tạo mã độc giả duy nhất
        var readerCode = await _readerCodeGenerator.GenerateUniqueCodeAsync(cancellationToken);

        var reader = Reader.Create(
            readerCode: readerCode,
            fullName: request.FullName,
            dateOfBirth: request.DateOfBirth,
            phoneNumber: request.PhoneNumber,
            registrationDate: request.RegistrationDate,
            expiryDate: request.ExpiryDate,
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
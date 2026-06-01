using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ONENET.Application.Common.Interfaces;
using ONENET.Domain.Common;
using ONENET.Domain.Entities;

namespace ONENET.Application.Readers.Commands;

public record CreateReaderCommand : IRequest<Result<Guid>>
{
    public string FullName { get; init; } = string.Empty;
    public DateTime DateOfBirth { get; init; }
    public string PhoneNumber { get; init; } = string.Empty;
    public string? Email { get; init; }
    public string? Address { get; init; }
    public DateTime? RegistrationDate { get; init; }
    public DateTime? ExpiryDate { get; init; }
}

public class CreateReaderCommandValidator : AbstractValidator<CreateReaderCommand>
{
    public CreateReaderCommandValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Họ tên độc giả không được để trống.")
            .MaximumLength(250).WithMessage("Họ tên không quá 250 ký tự.");

        RuleFor(x => x.DateOfBirth)
            .NotEmpty().WithMessage("Ngày sinh không được để trống.")
            .Must(dob => dob <= DateTime.UtcNow).WithMessage("Ngày sinh không được lớn hơn ngày hiện tại.");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Số điện thoại không được để trống.")
            .Matches(@"^0\d{9}$").WithMessage("Số điện thoại không hợp lệ (phải gồm 10 chữ số và bắt đầu bằng 0).");

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Email không hợp lệ.")
            .MaximumLength(250).WithMessage("Email không quá 250 ký tự.")
            .When(x => !string.IsNullOrEmpty(x.Email));

        RuleFor(x => x.Address)
            .MaximumLength(500).WithMessage("Địa chỉ không quá 500 ký tự.");

        RuleFor(x => x.ExpiryDate)
            .GreaterThanOrEqualTo(x => x.RegistrationDate ?? DateTime.UtcNow.Date)
            .WithMessage("Ngày hết hạn thẻ không được nhỏ hơn ngày đăng ký.")
            .When(x => x.ExpiryDate.HasValue);
    }
}

public class CreateReaderCommandHandler : IRequestHandler<CreateReaderCommand, Result<Guid>>
{
    private readonly IAppDbContext _context;

    public CreateReaderCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Guid>> Handle(CreateReaderCommand request, CancellationToken cancellationToken)
    {
        // Business Rule: Số điện thoại không được trùng
        var isPhoneExists = await _context.Readers
            .AnyAsync(r => r.PhoneNumber == request.PhoneNumber && !r.IsDeleted, cancellationToken);

        if (isPhoneExists)
        {
            return Result.Failure<Guid>($"Số điện thoại '{request.PhoneNumber}' đã tồn tại trong hệ thống.");
        }

        var regDate = request.RegistrationDate ?? DateTime.UtcNow;
        var expDate = request.ExpiryDate ?? regDate.AddYears(1);

        // Generate unique ReaderCode: DG[YYYYMMDD][SequentialNumber]
        var todayStr = regDate.ToString("yyyyMMdd");
        var readersCountToday = await _context.Readers
            .CountAsync(r => r.ReaderCode.StartsWith($"DG{todayStr}"), cancellationToken);

        var sequentialNumber = (readersCountToday + 1).ToString("D3"); // e.g. 001, 002
        var readerCode = $"DG{todayStr}{sequentialNumber}";

        // double check if generated reader code exists just in case
        while (await _context.Readers.AnyAsync(r => r.ReaderCode == readerCode, cancellationToken))
        {
            readersCountToday++;
            sequentialNumber = (readersCountToday + 1).ToString("D3");
            readerCode = $"DG{todayStr}{sequentialNumber}";
        }

        var reader = new Reader(
            readerCode,
            request.FullName,
            request.DateOfBirth,
            request.PhoneNumber,
            request.Email,
            request.Address,
            regDate,
            expDate
        );

        _context.Readers.Add(reader);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success(reader.Id);
    }
}

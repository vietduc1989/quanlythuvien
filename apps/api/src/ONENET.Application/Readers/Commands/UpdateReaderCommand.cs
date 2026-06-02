using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ONENET.Application.Common.Interfaces;
using ONENET.Domain.Common;
using ONENET.Domain.Enums;

namespace ONENET.Application.Readers.Commands;

public record UpdateReaderCommand : IRequest<Result>
{
    public Guid Id { get; init; }
    public string FullName { get; init; } = string.Empty;
    public DateTime DateOfBirth { get; init; }
    public string PhoneNumber { get; init; } = string.Empty;
    public string? Email { get; init; }
    public string? Address { get; init; }
    public ReaderStatus Status { get; init; }
    public DateTime ExpiryDate { get; init; }
}

public class UpdateReaderCommandValidator : AbstractValidator<UpdateReaderCommand>
{
    public UpdateReaderCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("ID độc giả không hợp lệ.");

        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Họ tên độc giả không được để trống.")
            .MaximumLength(250).WithMessage("Họ tên không quá 250 ký tự.");

        RuleFor(x => x.DateOfBirth)
            .NotEmpty().WithMessage("Ngày sinh không được để trống.")
            .Must(dob => dob <= DateTime.UtcNow).WithMessage("Ngày sinh không được lớn hơn ngày hiện tại.")
            .Must(dob => BeAtLeast16YearsOld(dob)).WithMessage("Độc giả phải từ 16 tuổi trở lên.");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Số điện thoại không được để trống.")
            .Matches(@"^0\d{9}$").WithMessage("Số điện thoại không hợp lệ (phải gồm 10 chữ số và bắt đầu bằng 0).");

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Email không hợp lệ.")
            .MaximumLength(250).WithMessage("Email không quá 250 ký tự.")
            .When(x => !string.IsNullOrEmpty(x.Email));

        RuleFor(x => x.Address)
            .MaximumLength(500).WithMessage("Địa chỉ không quá 500 ký tự.");
    }

    private static bool BeAtLeast16YearsOld(DateTime dob)
    {
        var age = DateTime.UtcNow.Year - dob.Year;
        if (dob.AddYears(16) > DateTime.UtcNow)
        {
            age--;
        }
        return age >= 16;
    }
}

public class UpdateReaderCommandHandler : IRequestHandler<UpdateReaderCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public UpdateReaderCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(UpdateReaderCommand request, CancellationToken cancellationToken)
    {
        var reader = await _context.Readers
            .FirstOrDefaultAsync(r => r.Id == request.Id && !r.IsDeleted, cancellationToken);

        if (reader == null)
        {
            return Result.Failure($"Không tìm thấy độc giả với ID '{request.Id}'.");
        }

        // Business Rule: Số điện thoại không được trùng với độc giả khác
        var isPhoneExists = await _context.Readers
            .AnyAsync(r => r.PhoneNumber == request.PhoneNumber && r.Id != request.Id && !r.IsDeleted, cancellationToken);

        if (isPhoneExists)
        {
            return Result.Failure($"Số điện thoại '{request.PhoneNumber}' đã tồn tại trong hệ thống ở tài khoản khác.");
        }

        // Business Rule: Độ tuổi phải từ 16 tuổi trở lên tính đến ngày đăng ký thẻ
        var age = reader.RegistrationDate.Year - request.DateOfBirth.Year;
        if (request.DateOfBirth.AddYears(16) > reader.RegistrationDate)
        {
            age--;
        }
        if (age < 16)
        {
            return Result.Failure("Độc giả phải từ 16 tuổi trở lên tính đến ngày đăng ký thẻ.");
        }

        reader.FullName = request.FullName;
        reader.DateOfBirth = request.DateOfBirth;
        reader.PhoneNumber = request.PhoneNumber;
        reader.Email = request.Email;
        reader.Address = request.Address;
        reader.Status = request.Status;
        reader.ExpiryDate = request.ExpiryDate;

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

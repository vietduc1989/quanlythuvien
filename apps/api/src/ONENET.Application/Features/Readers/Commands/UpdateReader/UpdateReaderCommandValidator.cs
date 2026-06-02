// QUAN-20260601-105011
using FluentValidation;
using ONENET.Application.Common.Interfaces;

namespace ONENET.Application.Features.Readers.Commands.UpdateReader;

/// <summary>
/// Validator cho UpdateReaderCommand.
/// </summary>
public class UpdateReaderCommandValidator : AbstractValidator<UpdateReaderCommand>
{
    private readonly IDateTime _dateTime;

    public UpdateReaderCommandValidator(IDateTime dateTime)
    {
        _dateTime = dateTime;

        RuleFor(x => x.ReaderId)
            .NotEmpty().WithMessage("Reader ID must not be empty.");

        RuleFor(x => x.FullName)
            .MaximumLength(250).WithMessage("'Full Name' must not exceed 250 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.FullName));

        RuleFor(x => x.DateOfBirth)
            .LessThan(_dateTime.Today).When(x => x.DateOfBirth.HasValue).WithMessage("'Date Of Birth' must be in the past.");

        RuleFor(x => x.PhoneNumber)
            .Matches(@"^0\d{9}$").When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber)).WithMessage("'Phone Number' must be 10 digits and start with 0.")
            .MaximumLength(15).When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber)).WithMessage("'Phone Number' must not exceed 15 characters.");

        RuleFor(x => x.Email)
            .MaximumLength(250).WithMessage("'Email' must not exceed 250 characters.")
            .EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Email)).WithMessage("Invalid email format.");

        RuleFor(x => x.Address)
            .MaximumLength(500).WithMessage("'Address' must not exceed 500 characters.");

        RuleFor(x => x.ExpiryDate)
            .GreaterThanOrEqualTo(x => x.RegistrationDate)
            .When(x => x.ExpiryDate.HasValue && x.RegistrationDate.HasValue)
            .WithMessage("'Expiry Date' must be greater than or equal to 'Registration Date'.");

        RuleFor(x => x.Status)
            .IsInEnum().When(x => x.Status.HasValue).WithMessage("Invalid 'Status' value.");
    }
}
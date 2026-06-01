// QUAN-20260601-105011
using FluentValidation;
using ONENET.Application.Common.Interfaces;

namespace ONENET.Application.Features.Readers.Commands.CreateReader;

/// <summary>
/// Validator cho CreateReaderCommand.
/// </summary>
public class CreateReaderCommandValidator : AbstractValidator<CreateReaderCommand>
{
    private readonly IDateTime _dateTime;

    public CreateReaderCommandValidator(IDateTime dateTime)
    {
        _dateTime = dateTime;

        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("'Full Name' must not be empty.")
            .MaximumLength(250).WithMessage("'Full Name' must not exceed 250 characters.");

        RuleFor(x => x.DateOfBirth)
            .NotEmpty().WithMessage("'Date Of Birth' must not be empty.")
            .LessThan(_dateTime.Today).WithMessage("'Date Of Birth' must be in the past.");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("'Phone Number' must not be empty.")
            .Matches(@"^0\d{9}$").WithMessage("'Phone Number' must be 10 digits and start with 0.")
            .MaximumLength(15).WithMessage("'Phone Number' must not exceed 15 characters."); // Max 15 per DB schema, but BR says 10 digits

        RuleFor(x => x.Email)
            .MaximumLength(250).WithMessage("'Email' must not exceed 250 characters.")
            .EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Email)).WithMessage("Invalid email format.");

        RuleFor(x => x.Address)
            .MaximumLength(500).WithMessage("'Address' must not exceed 500 characters.");

        RuleFor(x => x.RegistrationDate)
            .NotEmpty().WithMessage("'Registration Date' must not be empty.");

        RuleFor(x => x.ExpiryDate)
            .NotEmpty().WithMessage("'Expiry Date' must not be empty.")
            .GreaterThanOrEqualTo(x => x.RegistrationDate).WithMessage("'Expiry Date' must be greater than or equal to 'Registration Date'.");

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Invalid 'Status' value.");
    }
}
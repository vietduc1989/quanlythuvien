// QUAN-20260601-105011
using FluentValidation;
using ONENET.Domain.Enums;

namespace ONENET.Application.Features.Readers.Queries.GetReadersList;

/// <summary>
/// Validator cho GetReadersListQuery.
/// </summary>
public class GetReadersListQueryValidator : AbstractValidator<GetReadersListQuery>
{
    private static readonly string[] AllowedSortByFields = { "FullName", "ReaderCode", "RegistrationDate", "ExpiryDate" };
    private static readonly string[] AllowedSortOrders = { "Asc", "Desc" };

    public GetReadersListQueryValidator()
    {
        RuleFor(x => x.PageIndex)
            .GreaterThanOrEqualTo(1).WithMessage("'Page Index' must be at least 1.");

        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1).WithMessage("'Page Size' must be at least 1.")
            .LessThanOrEqualTo(100).WithMessage("'Page Size' must not exceed 100.");

        RuleFor(x => x.Status)
            .IsInEnum().When(x => x.Status.HasValue).WithMessage("Invalid 'Status' value.");

        RuleFor(x => x.SortBy)
            .Must(s => AllowedSortByFields.Contains(s, StringComparer.OrdinalIgnoreCase))
            .WithMessage($"'Sort By' must be one of: {string.Join(", ", AllowedSortByFields)}.")
            .When(x => !string.IsNullOrWhiteSpace(x.SortBy));

        RuleFor(x => x.SortOrder)
            .Must(s => AllowedSortOrders.Contains(s, StringComparer.OrdinalIgnoreCase))
            .WithMessage($"'Sort Order' must be one of: {string.Join(", ", AllowedSortOrders)}.")
            .When(x => !string.IsNullOrWhiteSpace(x.SortOrder));
    }
}
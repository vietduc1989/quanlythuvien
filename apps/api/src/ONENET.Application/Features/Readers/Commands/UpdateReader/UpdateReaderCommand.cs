// QUAN-20260601-105011
using MediatR;
using ONENET.Application.Common.Models;
using ONENET.Domain.Enums;

namespace ONENET.Application.Features.Readers.Commands.UpdateReader;

/// <summary>
/// Command để cập nhật thông tin của một độc giả hiện có.
/// </summary>
public record UpdateReaderCommand : IRequest<Result<Guid>>
{
    public Guid ReaderId { get; init; }
    public string? FullName { get; init; }
    public DateOnly? DateOfBirth { get; init; }
    public string? PhoneNumber { get; init; }
    public string? Email { get; init; }
    public string? Address { get; init; }
    public DateOnly? RegistrationDate { get; init; }
    public DateOnly? ExpiryDate { get; init; }
    public ReaderStatus? Status { get; init; }
    public DateTime? LastModifiedAtClient { get; init; } // Cho optimistic concurrency control
}
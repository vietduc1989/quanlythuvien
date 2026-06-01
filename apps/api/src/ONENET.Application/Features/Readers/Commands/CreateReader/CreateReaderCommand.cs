// QUAN-20260601-105011
using MediatR;
using ONENET.Application.Common.Models;
using ONENET.Domain.Enums;

namespace ONENET.Application.Features.Readers.Commands.CreateReader;

/// <summary>
/// Command để tạo mới một độc giả.
/// </summary>
public record CreateReaderCommand : IRequest<Result<CreateReaderResponse>>
{
    public string FullName { get; set; } = default!;
    public DateOnly DateOfBirth { get; set; }
    public string PhoneNumber { get; set; } = default!;
    public string? Email { get; set; }
    public string? Address { get; set; }
    public DateOnly RegistrationDate { get; set; }
    public DateOnly ExpiryDate { get; set; }
    public ReaderStatus Status { get; set; } = ReaderStatus.Active;
}

/// <summary>
/// Response khi tạo độc giả thành công.
/// </summary>
public record CreateReaderResponse(Guid ReaderId, string ReaderCode);
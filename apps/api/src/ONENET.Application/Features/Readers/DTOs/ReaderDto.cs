// QUAN-20260601-105011
using ONENET.Domain.Enums;

namespace ONENET.Application.Features.Readers.DTOs;

/// <summary>
/// DTO đầy đủ thông tin độc giả để trả về khi xem chi tiết.
/// </summary>
public class ReaderDto
{
    public Guid ReaderId { get; set; }
    public string ReaderCode { get; set; } = default!;
    public string FullName { get; set; } = default!;
    public DateOnly DateOfBirth { get; set; }
    public string PhoneNumber { get; set; } = default!;
    public string? Email { get; set; }
    public string? Address { get; set; }
    public DateOnly RegistrationDate { get; set; }
    public DateOnly ExpiryDate { get; set; }
    public ReaderStatus Status { get; set; }
}
// QUAN-20260601-105011
using ONENET.Domain.Enums;

namespace ONENET.Application.Features.Readers.DTOs;

/// <summary>
/// DTO rút gọn thông tin độc giả để hiển thị trong danh sách.
/// </summary>
public class ReaderListItemDto
{
    public Guid ReaderId { get; set; }
    public string ReaderCode { get; set; } = default!;
    public string FullName { get; set; } = default!;
    public string PhoneNumber { get; set; } = default!;
    public string? Email { get; set; }
    public ReaderStatus Status { get; set; }
    public DateOnly RegistrationDate { get; set; }
    public DateOnly ExpiryDate { get; set; }
}
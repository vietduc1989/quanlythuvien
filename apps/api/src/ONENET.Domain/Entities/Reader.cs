// QUAN-20260601-105011
using ONENET.Domain.Enums;

namespace ONENET.Domain.Entities;

/// <summary>
/// Đại diện cho một độc giả trong hệ thống thư viện.
/// Kế thừa BaseEntity để có các trường audit và soft delete.
/// </summary>
public class Reader : BaseEntity
{
    public string ReaderCode { get; private set; } = default!; // BR01: Mã độc giả duy nhất, tự sinh
    public string FullName { get; private set; } = default!;
    public DateOnly DateOfBirth { get; private set; }
    public string PhoneNumber { get; private set; } = default!; // BR02: Số điện thoại duy nhất
    public string? Email { get; private set; }
    public string? Address { get; private set; }
    public DateOnly RegistrationDate { get; private set; }
    public DateOnly ExpiryDate { get; private set; }
    public ReaderStatus Status { get; private set; }

    // Private constructor cho EF Core và để kiểm soát việc tạo entity qua factory method
    private Reader() { }

    /// <summary>
    /// Factory method để tạo một độc giả mới.
    /// Đảm bảo tính nhất quán của entity khi khởi tạo.
    /// </summary>
    public static Reader Create(
        string readerCode,
        string fullName,
        DateOnly dateOfBirth,
        string phoneNumber,
        DateOnly registrationDate,
        DateOnly expiryDate,
        string createdBy,
        string? email = null,
        string? address = null,
        ReaderStatus status = ReaderStatus.Active)
    {
        var reader = new Reader
        {
            ReaderCode = readerCode,
            FullName = fullName,
            DateOfBirth = dateOfBirth,
            PhoneNumber = phoneNumber,
            Email = email,
            Address = address,
            RegistrationDate = registrationDate,
            ExpiryDate = expiryDate,
            Status = status,
            CreatedBy = createdBy,
            LastModifiedBy = createdBy // Khi tạo, LastModifiedBy cũng là người tạo
        };
        // BaseEntity đã tự động thiết lập Id và CreatedAt
        return reader;
    }

    /// <summary>
    /// Cập nhật thông tin độc giả.
    /// </summary>
    public void Update(
        string? fullName,
        DateOnly? dateOfBirth,
        string? phoneNumber,
        string? email,
        string? address,
        DateOnly? registrationDate,
        DateOnly? expiryDate,
        ReaderStatus? status,
        string updatedBy)
    {
        if (!string.IsNullOrWhiteSpace(fullName))
        {
            FullName = fullName;
        }
        if (dateOfBirth.HasValue)
        {
            DateOfBirth = dateOfBirth.Value;
        }
        if (!string.IsNullOrWhiteSpace(phoneNumber))
        {
            PhoneNumber = phoneNumber;
        }
        Email = email; // Cho phép cập nhật thành null
        Address = address; // Cho phép cập nhật thành null
        if (registrationDate.HasValue)
        {
            RegistrationDate = registrationDate.Value;
        }
        if (expiryDate.HasValue)
        {
            ExpiryDate = expiryDate.Value;
        }
        if (status.HasValue)
        {
            Status = status.Value;
        }

        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = updatedBy;
    }

    /// <summary>
    /// Đánh dấu độc giả là đã xóa mềm.
    /// </summary>
    public void SoftDelete(string deletedBy)
    {
        IsDeleted = true;
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = deletedBy;
    }
}
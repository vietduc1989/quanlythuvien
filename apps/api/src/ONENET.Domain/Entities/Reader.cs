using System;
using ONENET.Domain.Common;
using ONENET.Domain.Enums;

namespace ONENET.Domain.Entities;

/// <summary>
/// Đại diện cho một độc giả trong hệ thống thư viện.
/// Kế thừa BaseEntity để có các trường audit và soft delete.
/// </summary>
public class Reader : BaseEntity
{
    public string ReaderCode { get; private set; } = string.Empty; // BR01: Mã độc giả duy nhất, tự sinh
    public string FullName { get; private set; } = string.Empty;
    public DateTime DateOfBirth { get; private set; }
    public string PhoneNumber { get; private set; } = string.Empty; // BR02: Số điện thoại duy nhất
    public string? Email { get; private set; }
    public string? Address { get; private set; }
    public DateTime RegistrationDate { get; private set; } = DateTime.UtcNow;
    public DateTime ExpiryDate { get; private set; } = DateTime.UtcNow.AddYears(1);
    public ReaderStatus Status { get; private set; } = ReaderStatus.Active;

    // Private constructor cho EF Core
    private Reader() { }

    /// <summary>
    /// Factory method để tạo một độc giả mới.
    /// </summary>
    public static Reader Create(
        string readerCode,
        string fullName,
        DateTime dateOfBirth,
        string phoneNumber,
        DateTime registrationDate,
        DateTime expiryDate,
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
            UpdatedBy = createdBy
        };
        return reader;
    }

    /// <summary>
    /// Cập nhật thông tin độc giả.
    /// </summary>
    public void Update(
        string? fullName,
        DateTime? dateOfBirth,
        string? phoneNumber,
        string? email,
        string? address,
        DateTime? registrationDate,
        DateTime? expiryDate,
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

        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    /// <summary>
    /// Đánh dấu độc giả là đã xóa mềm.
    /// </summary>
    public void SoftDelete(string deletedBy)
    {
        IsDeleted = true;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = deletedBy;
    }
}

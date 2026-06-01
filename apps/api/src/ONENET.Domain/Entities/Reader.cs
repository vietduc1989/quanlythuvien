using System;
using ONENET.Domain.Common;
using ONENET.Domain.Enums;

namespace ONENET.Domain.Entities;

public class Reader : BaseEntity
{
    public string ReaderCode { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Address { get; set; }
    public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;
    public DateTime ExpiryDate { get; set; } = DateTime.UtcNow.AddYears(1);
    public ReaderStatus Status { get; set; } = ReaderStatus.Active;

    public Reader()
    {
    }

    public Reader(string readerCode, string fullName, DateTime dateOfBirth, string phoneNumber, string? email, string? address, DateTime registrationDate, DateTime expiryDate)
    {
        ReaderCode = readerCode;
        FullName = fullName;
        DateOfBirth = dateOfBirth;
        PhoneNumber = phoneNumber;
        Email = email;
        Address = address;
        RegistrationDate = registrationDate;
        ExpiryDate = expiryDate;
        Status = ReaderStatus.Active;
    }
}

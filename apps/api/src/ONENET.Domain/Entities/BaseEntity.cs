// QUAN-20260601-105011
using System;

namespace ONENET.Domain.Entities;

/// <summary>
/// Lớp cơ sở cho các entity trong hệ thống, bao gồm các trường audit và soft delete.
/// </summary>
public abstract class BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; } = false;
}

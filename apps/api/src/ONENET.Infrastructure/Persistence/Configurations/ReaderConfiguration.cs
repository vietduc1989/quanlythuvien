// QUAN-20260601-105011
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ONENET.Domain.Entities;

namespace ONENET.Infrastructure.Persistence.Configurations;

/// <summary>
/// Cấu hình Fluent API cho entity Reader trong Entity Framework Core.
/// </summary>
public class ReaderConfiguration : IEntityTypeConfiguration<Reader>
{
    public void Configure(EntityTypeBuilder<Reader> builder)
    {
        builder.ToTable("readers");

        // Primary Key
        builder.HasKey(r => r.Id);

        // Cấu hình các thuộc tính
        builder.Property(r => r.ReaderCode)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(r => r.ReaderCode)
            .IsUnique();

        builder.Property(r => r.FullName)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(r => r.DateOfBirth)
            .IsRequired();

        builder.Property(r => r.PhoneNumber)
            .IsRequired()
            .HasMaxLength(15);

        builder.HasIndex(r => r.PhoneNumber)
            .IsUnique();

        builder.Property(r => r.Email)
            .HasMaxLength(250);

        builder.Property(r => r.Address)
            .HasMaxLength(500);

        builder.Property(r => r.RegistrationDate)
            .IsRequired();

        builder.Property(r => r.ExpiryDate)
            .IsRequired();

        builder.Property(r => r.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        // Map UpdatedAt and UpdatedBy to database columns LastModifiedAt and LastModifiedBy
        builder.Property(r => r.UpdatedAt)
            .HasColumnName("LastModifiedAt");

        builder.Property(r => r.UpdatedBy)
            .HasColumnName("LastModifiedBy");

        // Soft Delete: Global Query Filter
        builder.HasQueryFilter(r => !r.IsDeleted);
    }
}

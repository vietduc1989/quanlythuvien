// QUAN-20260601-105011
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ONENET.Domain.Entities;
using ONENET.Domain.Enums;

namespace ONENET.Infrastructure.Persistence.Configurations;

/// <summary>
/// Cấu hình Fluent API cho entity Reader trong Entity Framework Core.
/// </summary>
public class ReaderConfiguration : IEntityTypeConfiguration<Reader>
{
    public void Configure(EntityTypeBuilder<Reader> builder)
    {
        // Tên bảng theo convention (snake_case, số nhiều)
        builder.ToTable("readers");

        // Primary Key
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd(); // Id (Guid) tự động sinh khi thêm mới

        // Cấu hình các thuộc tính
        builder.Property(r => r.ReaderCode)
            .HasColumnName("reader_code")
            .IsRequired()
            .HasMaxLength(20);
        builder.HasIndex(r => r.ReaderCode)
            .IsUnique(); // UNIQUE INDEX (ReaderCode)

        builder.Property(r => r.FullName)
            .HasColumnName("full_name")
            .IsRequired()
            .HasMaxLength(250);
        // INDEX (FullName) sẽ được tạo tự động hoặc có thể chỉ định rõ ràng
        // builder.HasIndex(r => r.FullName); // Được đề xuất trong BRD

        builder.Property(r => r.DateOfBirth)
            .HasColumnName("date_of_birth")
            .IsRequired();

        builder.Property(r => r.PhoneNumber)
            .HasColumnName("phone_number")
            .IsRequired()
            .HasMaxLength(15);
        builder.HasIndex(r => r.PhoneNumber)
            .IsUnique(); // UNIQUE INDEX (PhoneNumber)

        builder.Property(r => r.Email)
            .HasColumnName("email")
            .HasMaxLength(250);

        builder.Property(r => r.Address)
            .HasColumnName("address")
            .HasMaxLength(500);

        builder.Property(r => r.RegistrationDate)
            .HasColumnName("registration_date")
            .IsRequired();

        builder.Property(r => r.ExpiryDate)
            .HasColumnName("expiry_date")
            .IsRequired();

        builder.Property(r => r.Status)
            .HasColumnName("status")
            .IsRequired()
            .HasConversion<string>() // Lưu enum dưới dạng string trong DB
            .HasMaxLength(50);
        // INDEX (Status, ExpiryDate) được đề xuất trong BRD
        // builder.HasIndex(r => new { r.Status, r.ExpiryDate });

        // Cấu hình các trường audit (kế thừa từ BaseEntity)
        builder.Property(r => r.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(r => r.CreatedBy).HasColumnName("created_by").IsRequired().HasMaxLength(255);
        builder.Property(r => r.LastModifiedAt).HasColumnName("last_modified_at");
        builder.Property(r => r.LastModifiedBy).HasColumnName("last_modified_by").HasMaxLength(255);
        builder.Property(r => r.IsDeleted).HasColumnName("is_deleted").IsRequired();

        // Soft Delete: Global Query Filter
        builder.HasQueryFilter(r => !r.IsDeleted);

        // PostgreSQL xmin for concurrency:
        // EF Core does not directly map xmin as a concurrency token.
        // The application logic relies on LastModifiedAt for optimistic concurrency.
        // If explicit xmin-based concurrency is needed, a custom interceptor/trigger
        // would be required. For now, LastModifiedAt is sufficient per BRD's "simple cases" note.
    }
}
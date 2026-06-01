using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ONENET.Domain.Entities;

namespace ONENET.Infrastructure.Persistence.Configurations;

public class ReaderConfiguration : IEntityTypeConfiguration<Reader>
{
    public void Configure(EntityTypeBuilder<Reader> builder)
    {
        builder.ToTable("readers");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
            .HasColumnName("id");

        builder.Property(r => r.ReaderCode)
            .HasColumnName("reader_code")
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(r => r.ReaderCode)
            .IsUnique();

        builder.Property(r => r.FullName)
            .HasColumnName("full_name")
            .HasMaxLength(250)
            .IsRequired();

        builder.HasIndex(r => r.FullName);

        builder.Property(r => r.DateOfBirth)
            .HasColumnName("date_of_birth")
            .IsRequired();

        builder.Property(r => r.PhoneNumber)
            .HasColumnName("phone_number")
            .HasMaxLength(15)
            .IsRequired();

        builder.HasIndex(r => r.PhoneNumber)
            .IsUnique();

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
            .HasMaxLength(50)
            .HasConversion<string>()
            .IsRequired();

        builder.HasIndex(r => new { r.Status, r.ExpiryDate });

        // Audit properties mapping
        builder.Property(r => r.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(r => r.CreatedBy)
            .HasColumnName("created_by")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(r => r.LastModifiedAt)
            .HasColumnName("last_modified_at");

        builder.Property(r => r.LastModifiedBy)
            .HasColumnName("last_modified_by")
            .HasMaxLength(100);

        builder.Property(r => r.IsDeleted)
            .HasColumnName("is_deleted")
            .IsRequired()
            .HasDefaultValue(false);

        // Global query filter to exclude soft deleted readers
        builder.HasQueryFilter(r => !r.IsDeleted);
    }
}

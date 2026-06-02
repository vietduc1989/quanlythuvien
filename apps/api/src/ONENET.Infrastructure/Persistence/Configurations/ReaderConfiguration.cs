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

        builder.Property(r => r.ReaderCode)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(r => r.ReaderCode)
            .IsUnique();

        builder.Property(r => r.FullName)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(r => r.PhoneNumber)
            .IsRequired()
            .HasMaxLength(15);

        builder.HasIndex(r => r.PhoneNumber)
            .IsUnique();

        builder.Property(r => r.Email)
            .HasMaxLength(250);

        builder.Property(r => r.Address)
            .HasMaxLength(500);

        builder.Property(r => r.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        // Soft delete global filter
        builder.HasQueryFilter(r => !r.IsDeleted);
    }
}

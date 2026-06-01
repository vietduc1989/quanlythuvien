using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ONENET.Domain.Entities;

namespace ONENET.Infrastructure.Persistence.Configurations;

public class BookConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.ToTable("books");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.Id)
            .HasColumnName("id");

        builder.Property(b => b.BookCode)
            .HasColumnName("book_code")
            .HasMaxLength(50)
            .IsRequired();

        // Unique index on BookCode for non-deleted books
        builder.HasIndex(b => b.BookCode)
            .IsUnique();

        builder.Property(b => b.Title)
            .HasColumnName("title")
            .HasMaxLength(200)
            .IsRequired();

        builder.HasIndex(b => b.Title);

        builder.Property(b => b.Author)
            .HasColumnName("author")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(b => b.Category)
            .HasColumnName("category")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(b => b.Publisher)
            .HasColumnName("publisher")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(b => b.PublishYear)
            .HasColumnName("publish_year")
            .IsRequired();

        builder.Property(b => b.Quantity)
            .HasColumnName("quantity")
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(b => b.ShelfLocation)
            .HasColumnName("shelf_location")
            .HasMaxLength(50);

        builder.Property(b => b.Status)
            .HasColumnName("status")
            .HasMaxLength(20)
            .HasConversion<string>()
            .IsRequired();

        // Audit & Soft Delete properties configuration
        builder.Property(b => b.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(b => b.CreatedBy)
            .HasColumnName("created_by")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(b => b.LastModifiedAt)
            .HasColumnName("last_modified_at");

        builder.Property(b => b.LastModifiedBy)
            .HasColumnName("last_modified_by")
            .HasMaxLength(100);

        builder.Property(b => b.IsDeleted)
            .HasColumnName("is_deleted")
            .IsRequired()
            .HasDefaultValue(false);

        // Global query filter to exclude soft-deleted books
        builder.HasQueryFilter(b => !b.IsDeleted);
    }
}

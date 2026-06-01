using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ONENET.Domain.Entities;

namespace ONENET.Infrastructure.Persistence.Configurations;

public class BookConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.ToTable("books");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id");

        builder.Property(x => x.BookCode)
            .HasColumnName("book_code")
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(x => x.BookCode)
            .IsUnique();

        builder.Property(x => x.Title)
            .HasColumnName("title")
            .HasMaxLength(200)
            .IsRequired();

        builder.HasIndex(x => x.Title);

        builder.Property(x => x.Author)
            .HasColumnName("author")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Category)
            .HasColumnName("category")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Publisher)
            .HasColumnName("publisher")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.PublishYear)
            .HasColumnName("publish_year")
            .IsRequired();

        builder.Property(x => x.Quantity)
            .HasColumnName("quantity")
            .HasDefaultValue(0)
            .IsRequired();

        builder.Property(x => x.ShelfLocation)
            .HasColumnName("shelf_location")
            .HasMaxLength(50);

        builder.Property(x => x.Status)
            .HasColumnName("status")
            .HasMaxLength(20)
            .HasConversion<string>()
            .IsRequired();

        // Audit properties mapping
        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(x => x.CreatedBy)
            .HasColumnName("created_by")
            .HasMaxLength(100);

        builder.Property(x => x.UpdatedAt)
            .HasColumnName("updated_at");

        builder.Property(x => x.UpdatedBy)
            .HasColumnName("updated_by")
            .HasMaxLength(100);

        builder.Property(x => x.IsDeleted)
            .HasColumnName("is_deleted")
            .HasDefaultValue(false)
            .IsRequired();

        // Concurrency token using xmin in PostgreSQL
        builder.Property<uint>("xmin")
            .HasColumnName("xmin")
            .HasColumnType("xid")
            .ValueGeneratedOnAddOrUpdate()
            .IsConcurrencyToken();

        // Global query filter
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}

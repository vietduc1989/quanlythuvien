using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ONENET.Domain.Entities;

namespace ONENET.Infrastructure.Persistence.Configurations;

public class PhieuMuonConfiguration : IEntityTypeConfiguration<PhieuMuon>
{
    public void Configure(EntityTypeBuilder<PhieuMuon> builder)
    {
        builder.ToTable("phieu_muons");

        builder.HasKey(pm => pm.Id);

        builder.Property(pm => pm.DocGiaId)
            .IsRequired();

        builder.HasIndex(pm => pm.DocGiaId);
        builder.HasIndex(pm => pm.NgayHenTra);
        builder.HasIndex(pm => pm.TrangThaiPhieuMuon);

        builder.Property(pm => pm.TrangThaiPhieuMuon)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        // Relation PhieuMuon (1) - (n) ChiTietPhieuMuon
        builder.HasMany(pm => pm.ChiTietPhieuMuons)
            .WithOne(ct => ct.PhieuMuon)
            .HasForeignKey(ct => ct.PhieuMuonId)
            .OnDelete(DeleteBehavior.Cascade);

        // Soft delete global filter
        builder.HasQueryFilter(pm => !pm.IsDeleted);
    }
}

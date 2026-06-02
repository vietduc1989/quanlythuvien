using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ONENET.Domain.Entities;

namespace ONENET.Infrastructure.Persistence.Configurations;

public class ChiTietPhieuMuonConfiguration : IEntityTypeConfiguration<ChiTietPhieuMuon>
{
    public void Configure(EntityTypeBuilder<ChiTietPhieuMuon> builder)
    {
        builder.ToTable("chi_tiet_phieu_muons");

        builder.HasKey(ct => ct.Id);

        builder.Property(ct => ct.PhieuMuonId)
            .IsRequired();

        builder.Property(ct => ct.SachId)
            .IsRequired();

        builder.HasIndex(ct => ct.PhieuMuonId);
        builder.HasIndex(ct => ct.SachId);
        builder.HasIndex(ct => ct.TrangThaiChiTiet);
        builder.HasIndex(ct => ct.IsQuaHan);

        builder.Property(ct => ct.TrangThaiChiTiet)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        // Soft delete global filter
        builder.HasQueryFilter(ct => !ct.IsDeleted);
    }
}

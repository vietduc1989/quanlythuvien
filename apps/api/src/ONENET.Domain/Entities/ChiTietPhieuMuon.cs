using System;

namespace ONENET.Domain.Entities;

public class ChiTietPhieuMuon : BaseEntity
{
    public Guid PhieuMuonId { get; set; }
    public Guid SachId { get; set; }
    public DateTime? NgayTraThucTe { get; set; }
    public string TrangThaiChiTiet { get; set; } = "DangMuon"; // DangMuon, DaTra
    public bool IsQuaHan { get; set; }
}

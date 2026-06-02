using System;
using System.Collections.Generic;
using ONENET.Domain.Common;
using ONENET.Domain.Enums;

namespace ONENET.Domain.Entities;

public class PhieuMuon : BaseEntity
{
    public Guid DocGiaId { get; set; }
    public DateTime NgayMuon { get; set; } = DateTime.UtcNow;
    public DateTime NgayHenTra { get; set; } = DateTime.UtcNow.AddDays(14);
    public LoanStatus TrangThaiPhieuMuon { get; set; } = LoanStatus.DangMuon;

    public ICollection<ChiTietPhieuMuon> ChiTietPhieuMuons { get; set; } = new List<ChiTietPhieuMuon>();

    public PhieuMuon()
    {
    }

    public PhieuMuon(Guid docGiaId, DateTime ngayMuon, DateTime ngayHenTra)
    {
        DocGiaId = docGiaId;
        NgayMuon = ngayMuon;
        NgayHenTra = ngayHenTra;
        TrangThaiPhieuMuon = LoanStatus.DangMuon;
    }
}

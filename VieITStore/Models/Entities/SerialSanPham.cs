using System.ComponentModel.DataAnnotations;

namespace VieITStore.Models.Entities;

public enum TrangThaiSerial
{
    TrongKho = 1,
    DaBan = 2,
    DangBaoHanh = 3,
    NgungTheoDoi = 4
}

public class SerialSanPham
{
    public int Id { get; set; }

    [Required, MaxLength(150)]
    public string MaSerial { get; set; } = string.Empty;

    public int SanPhamId { get; set; }
    public virtual SanPham? SanPham { get; set; }

    public int? ChiTietPhieuNhapId { get; set; }
    public virtual ChiTietPhieuNhap? ChiTietPhieuNhap { get; set; }

    public int? ChiTietDonHangId { get; set; }
    public virtual ChiTietDonHang? ChiTietDonHang { get; set; }

    public TrangThaiSerial TrangThai { get; set; } = TrangThaiSerial.TrongKho;
    public DateTime? NgayBatDauBaoHanh { get; set; }
    public DateTime? NgayHetHanBaoHanh { get; set; }
    public DateTime NgayTao { get; set; } = DateTime.Now;

    [MaxLength(500)]
    public string? GhiChu { get; set; }
}

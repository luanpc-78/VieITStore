using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VieITStore.Models.Entities
{
    public enum TrangThaiDonHang
    {
        ChoXacNhan = 1,
        DaXacNhan = 2,
        DangGiao = 3,
        DaGiao = 4,
        HoanThanh = 5,
        DaHuy = 6,
        YeuCauTraHang = 7
    }

    public enum PhuongThucThanhToan
    {
        COD = 1,
        ChuyenKhoan = 2,
        ViDienTu = 3,
        TheTinDung = 4
    }

    public class DonHang
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string MaDonHang { get; set; } = string.Empty;

        [Required]
        public int KhachHangId { get; set; }
        public virtual KhachHang? KhachHang { get; set; }

        public DateTime NgayDat { get; set; } = DateTime.Now;
        public DateTime? NgayGiaoDuKien { get; set; }
        public DateTime? NgayGiaoThucTe { get; set; }
        public TrangThaiDonHang TrangThai { get; set; } = TrangThaiDonHang.ChoXacNhan;
        public PhuongThucThanhToan PhuongThucThanhToan { get; set; }
        public bool DaThanhToan { get; set; } = false;
        public DateTime? NgayThanhToan { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TongTienHang { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal PhiVanChuyen { get; set; } = 30000;

        [Column(TypeName = "decimal(18,2)")]
        public decimal GiamGia { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TongThanhToan { get; set; }

        public string HoTenNguoiNhan { get; set; } = string.Empty;
        public string SoDienThoaiNguoiNhan { get; set; } = string.Empty;
        public string DiaChiGiaoHang { get; set; } = string.Empty;
        public string? GhiChu { get; set; }
        public string? LyDoHuy { get; set; }

        public int? NhanVienXuLyId { get; set; }
        public virtual NguoiDung? NhanVienXuLy { get; set; }

        public virtual ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; } = new List<ChiTietDonHang>();
        public virtual HoaDon? HoaDon { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VieITStore.Models.Entities
{
    public partial class SanPham
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string MaSanPham { get; set; } = string.Empty;

        [Required]
        public string TenSanPham { get; set; } = string.Empty;

        public string? Slug { get; set; }
        public string? MoTaNgan { get; set; }
        public string? MoTaChiTiet { get; set; }

        [Required]
        public int DanhMucId { get; set; }
        public virtual DanhMuc? DanhMuc { get; set; }

        public string? ThuongHieu { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal GiaNhap { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal GiaBan { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? GiaKhuyenMai { get; set; }

        public int SoLuongTon { get; set; }
        public string DonViTinh { get; set; } = "Cái";
        public string? HinhAnh { get; set; }
        public int BaoHanh { get; set; }
        public string? XuatXu { get; set; }
        public int SoLuongDaBan { get; set; }
        public bool NoiBat { get; set; } = false;
        public bool TrangThai { get; set; } = true;
        public DateTime NgayTao { get; set; } = DateTime.Now;
        public DateTime NgayCapNhat { get; set; } = DateTime.Now;

        public virtual ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; } = new List<ChiTietDonHang>();
        public virtual ICollection<DanhGia> DanhGias { get; set; } = new List<DanhGia>();
        public virtual ICollection<ChiTietPhieuNhap> ChiTietPhieuNhaps { get; set; } = new List<ChiTietPhieuNhap>();
        public virtual ICollection<SerialSanPham> SerialSanPhams { get; set; } = new List<SerialSanPham>();
    }
}

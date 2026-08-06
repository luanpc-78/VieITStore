using System.ComponentModel.DataAnnotations;

namespace VieITStore.Models.Entities
{
    public enum VaiTro
    {
        Admin = 1,
        NhanVien = 2,
        KhachHang = 3
    }

    public class NguoiDung
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string TenDangNhap { get; set; } = string.Empty;

        [Required]
        public string MatKhauHash { get; set; } = string.Empty;

        [Required]
        public string HoTen { get; set; } = string.Empty;

        public string? Email { get; set; }
        public string? SoDienThoai { get; set; }
        public VaiTro VaiTro { get; set; } = VaiTro.KhachHang;
        public bool TrangThai { get; set; } = true;
        public DateTime NgayTao { get; set; } = DateTime.Now;
        public DateTime? LanDangNhapCuoi { get; set; }

        public virtual KhachHang? KhachHang { get; set; }
    }
}
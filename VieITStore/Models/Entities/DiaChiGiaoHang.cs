using System.ComponentModel.DataAnnotations;

namespace VieITStore.Models.Entities
{
    public class DiaChiGiaoHang
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int KhachHangId { get; set; }
        public virtual KhachHang? KhachHang { get; set; }

        [Required]
        public string HoTenNguoiNhan { get; set; } = string.Empty;

        [Required]
        public string SoDienThoai { get; set; } = string.Empty;

        [Required]
        public string TinhThanh { get; set; } = string.Empty;

        [Required]
        public string QuanHuyen { get; set; } = string.Empty;

        [Required]
        public string PhuongXa { get; set; } = string.Empty;

        [Required]
        public string DiaChiChiTiet { get; set; } = string.Empty;

        public bool LaMacDinh { get; set; } = false;
        public string? GhiChu { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;

namespace VieITStore.Models.Entities
{
    public class NhaCungCap
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string MaNCC { get; set; } = string.Empty;

        [Required]
        public string TenNCC { get; set; } = string.Empty;

        public string? NguoiLienHe { get; set; }
        public string? SoDienThoai { get; set; }
        public string? Email { get; set; }
        public string? DiaChi { get; set; }
        public string? MaSoThue { get; set; }
        public bool TrangThai { get; set; } = true;
    }
}
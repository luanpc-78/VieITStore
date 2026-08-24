using System.ComponentModel.DataAnnotations;

namespace VieITStore.Models.Entities
{
    public class DanhGia
    {
        [Key]
        public int Id { get; set; }

        public int SanPhamId { get; set; }
        public virtual SanPham? SanPham { get; set; }

        public int KhachHangId { get; set; }
        public virtual KhachHang? KhachHang { get; set; }

        [Range(1, 5)]
        public int SoSao { get; set; }

        [Required, StringLength(1000, MinimumLength = 10)]
        public string NoiDung { get; set; } = string.Empty;
        public DateTime NgayDanhGia { get; set; } = DateTime.Now;
        public DateTime? NgayCapNhat { get; set; }
        public bool HienThi { get; set; } = true;
    }
}

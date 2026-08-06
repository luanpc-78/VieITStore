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

        public string? NoiDung { get; set; }
        public DateTime NgayDanhGia { get; set; } = DateTime.Now;
        public bool HienThi { get; set; } = true;
    }
}
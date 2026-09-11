using System.ComponentModel.DataAnnotations.Schema;

using System.ComponentModel.DataAnnotations;

namespace VieITStore.Models.Entities
{
    public class GioHang
    {
        public int Id { get; set; }
        public int? NguoiDungId { get; set; }
        public virtual NguoiDung? NguoiDung { get; set; }
        [MaxLength(100)]
        public string? SessionId { get; set; }
        public int SanPhamId { get; set; }
        public virtual SanPham? SanPham { get; set; }
        public int SoLuong { get; set; }

        [NotMapped]
        public decimal ThanhTien => (SanPham?.GiaKhuyenMai ?? SanPham?.GiaBan ?? 0) * SoLuong;
    }
}

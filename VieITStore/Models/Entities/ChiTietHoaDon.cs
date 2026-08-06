using System.ComponentModel.DataAnnotations.Schema;

namespace VieITStore.Models.Entities
{
    public class ChiTietHoaDon
    {
        public int Id { get; set; }
        public int HoaDonId { get; set; }
        public virtual HoaDon? HoaDon { get; set; }
        public int SanPhamId { get; set; }
        public virtual SanPham? SanPham { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal DonGia { get; set; }

        public int SoLuong { get; set; }

        [NotMapped]
        public decimal ThanhTien => DonGia * SoLuong;
    }
}
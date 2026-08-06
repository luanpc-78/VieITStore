using System.ComponentModel.DataAnnotations.Schema;

namespace VieITStore.Models.Entities
{
    public class ChiTietDonHang
    {
        public int Id { get; set; }
        public int DonHangId { get; set; }
        public virtual DonHang? DonHang { get; set; }
        public int SanPhamId { get; set; }
        public virtual SanPham? SanPham { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal DonGia { get; set; }

        public int SoLuong { get; set; }

        [NotMapped]
        public decimal ThanhTien => DonGia * SoLuong;
    }
}

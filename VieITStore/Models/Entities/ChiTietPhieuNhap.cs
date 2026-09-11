using System.ComponentModel.DataAnnotations.Schema;

namespace VieITStore.Models.Entities
{
    public class ChiTietPhieuNhap
    {
        public int Id { get; set; }
        public int PhieuNhapId { get; set; }
        public virtual PhieuNhap? PhieuNhap { get; set; }
        public int SanPhamId { get; set; }
        public virtual SanPham? SanPham { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal DonGiaNhap { get; set; }

        public int SoLuong { get; set; }

        public virtual ICollection<SerialSanPham> SerialSanPhams { get; set; } = new List<SerialSanPham>();

        [NotMapped]
        public decimal ThanhTien => DonGiaNhap * SoLuong;
    }
}

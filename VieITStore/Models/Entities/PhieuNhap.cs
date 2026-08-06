using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VieITStore.Models.Entities
{
    public class PhieuNhap
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string MaPhieuNhap { get; set; } = string.Empty;

        public int NhaCungCapId { get; set; }
        public virtual NhaCungCap? NhaCungCap { get; set; }

        public DateTime NgayNhap { get; set; } = DateTime.Now;

        public int NguoiNhapId { get; set; }
        public virtual NguoiDung? NguoiNhap { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TongTien { get; set; }

        public string? GhiChu { get; set; }

        public virtual ICollection<ChiTietPhieuNhap> ChiTietPhieuNhaps { get; set; } = new List<ChiTietPhieuNhap>();
    }
}
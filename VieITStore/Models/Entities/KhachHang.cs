using System.ComponentModel.DataAnnotations;

namespace VieITStore.Models.Entities
{
    public class KhachHang
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int NguoiDungId { get; set; }
        public virtual NguoiDung? NguoiDung { get; set; }

        public string? GioiTinh { get; set; }
        public DateTime? NgaySinh { get; set; }
        public string? MaSoThue { get; set; }
        public string? TenCongTy { get; set; }

        public virtual ICollection<DiaChiGiaoHang> DiaChiGiaoHangs { get; set; } = new List<DiaChiGiaoHang>();
        public virtual ICollection<DonHang> DonHangs { get; set; } = new List<DonHang>();
        public virtual ICollection<DanhGia> DanhGias { get; set; } = new List<DanhGia>();
    }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VieITStore.Models.Entities
{
    public class MaGiamGia
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string MaCode { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string TenKhuyenMai { get; set; } = string.Empty;

        public string? MoTa { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal PhanTramGiam { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? GiaGiamToiDa { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? GiaToiThieu { get; set; }

        public DateTime NgayBatDau { get; set; }

        public DateTime NgayKetThuc { get; set; }

        public int? SoLuongSuDung { get; set; }

        public int SoLanDaSuDung { get; set; } = 0;

        public bool ApDungToanBo { get; set; } = true;

        public int? DanhMucId { get; set; }
        public virtual DanhMuc? DanhMuc { get; set; }

        public bool TrangThai { get; set; } = true;

        public DateTime NgayTao { get; set; } = DateTime.Now;

        public DateTime NgayCapNhat { get; set; } = DateTime.Now;

        public virtual ICollection<DanhMuc> DanhMucs { get; set; } = new List<DanhMuc>();
    }
}

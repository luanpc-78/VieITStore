using System.ComponentModel.DataAnnotations;

namespace VieITStore.Models.Entities
{
    public class DanhMuc
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string TenDanhMuc { get; set; } = string.Empty;

        public string? MoTa { get; set; }
        public int ThuTu { get; set; }
        public bool HienThi { get; set; } = true;

        public virtual ICollection<SanPham> SanPhams { get; set; } = new List<SanPham>();
    }
}
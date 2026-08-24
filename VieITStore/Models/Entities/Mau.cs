using System.ComponentModel.DataAnnotations;

namespace VieITStore.Models.Entities
{
    public class Mau
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string TenMau { get; set; } = string.Empty;

        [MaxLength(7)]
        public string? MaMau { get; set; }

        public bool TrangThai { get; set; } = true;

        public DateTime NgayTao { get; set; } = DateTime.Now;

        public virtual ICollection<SanPham> SanPhams { get; set; } = new List<SanPham>();
    }
}

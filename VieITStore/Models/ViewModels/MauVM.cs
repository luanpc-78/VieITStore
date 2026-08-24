using System.ComponentModel.DataAnnotations;

namespace VieITStore.Models.ViewModels
{
    public class MauVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên màu không được để trống")]
        [MaxLength(50)]
        public string TenMau { get; set; } = string.Empty;

        [MaxLength(7)]
        [RegularExpression("^#[0-9A-Fa-f]{6}$", ErrorMessage = "Mã màu phải là mã HEX (ví dụ: #FF5733)")]
        public string? MaMau { get; set; }

        public bool TrangThai { get; set; } = true;
    }
}

using System.ComponentModel.DataAnnotations;
using VieITStore.Models.Entities;

namespace VieITStore.Models.ViewModels;

public class HoSoVM
{
    [Required(ErrorMessage = "Vui lòng nhập họ tên")]
    public string HoTen { get; set; } = string.Empty;

    [Required, EmailAddress(ErrorMessage = "Email không hợp lệ")]
    public string Email { get; set; } = string.Empty;

    [Required, Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
    public string SoDienThoai { get; set; } = string.Empty;

    public string? GioiTinh { get; set; }

    [DataType(DataType.Date)]
    public DateTime? NgaySinh { get; set; }

    public List<DiaChiGiaoHang> DiaChis { get; set; } = [];
}

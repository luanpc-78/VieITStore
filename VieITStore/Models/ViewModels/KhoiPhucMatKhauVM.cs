using System.ComponentModel.DataAnnotations;

namespace VieITStore.Models.ViewModels;

public class QuenMatKhauVM
{
    [Display(Name = "Email")]
    [Required(ErrorMessage = "Vui lòng nhập email")]
    [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
    public string Email { get; set; } = string.Empty;
}

public class DatLaiMatKhauVM
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "Mã xác nhận")]
    [Required(ErrorMessage = "Vui lòng nhập mã xác nhận")]
    [StringLength(6, MinimumLength = 6, ErrorMessage = "Mã xác nhận phải gồm 6 chữ số")]
    public string MaXacNhan { get; set; } = string.Empty;

    [Display(Name = "Mật khẩu mới")]
    [Required(ErrorMessage = "Vui lòng nhập mật khẩu mới")]
    [MinLength(8, ErrorMessage = "Mật khẩu phải có ít nhất 8 ký tự")]
    [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d).{8,}$", ErrorMessage = "Mật khẩu phải có ít nhất một chữ cái và một chữ số")]
    [DataType(DataType.Password)]
    public string MatKhauMoi { get; set; } = string.Empty;

    [Display(Name = "Xác nhận mật khẩu")]
    [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu")]
    [Compare(nameof(MatKhauMoi), ErrorMessage = "Mật khẩu xác nhận không khớp")]
    [DataType(DataType.Password)]
    public string XacNhanMatKhau { get; set; } = string.Empty;
}

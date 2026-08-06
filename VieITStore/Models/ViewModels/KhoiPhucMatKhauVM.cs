using System.ComponentModel.DataAnnotations;

namespace VieITStore.Models.ViewModels;

public class QuenMatKhauVM
{
    [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập hoặc email")]
    public string TaiKhoan { get; set; } = string.Empty;
}

public class DatLaiMatKhauVM
{
    [Required]
    public string TaiKhoan { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập mã xác nhận")]
    [StringLength(6, MinimumLength = 6, ErrorMessage = "Mã xác nhận phải gồm 6 chữ số")]
    public string MaXacNhan { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập mật khẩu mới")]
    [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
    [DataType(DataType.Password)]
    public string MatKhauMoi { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu")]
    [Compare(nameof(MatKhauMoi), ErrorMessage = "Mật khẩu xác nhận không khớp")]
    [DataType(DataType.Password)]
    public string XacNhanMatKhau { get; set; } = string.Empty;
}

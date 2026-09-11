using System.ComponentModel.DataAnnotations;

namespace VieITStore.Models.ViewModels;

public class NhaCungCapVM
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập mã nhà cung cấp.")]
    [StringLength(50, ErrorMessage = "Mã nhà cung cấp không được vượt quá 50 ký tự.")]
    [Display(Name = "Mã nhà cung cấp")]
    public string MaNCC { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập tên nhà cung cấp.")]
    [StringLength(200, ErrorMessage = "Tên nhà cung cấp không được vượt quá 200 ký tự.")]
    [Display(Name = "Tên nhà cung cấp")]
    public string TenNCC { get; set; } = string.Empty;

    [StringLength(150)]
    [Display(Name = "Người liên hệ")]
    public string? NguoiLienHe { get; set; }

    [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
    [RegularExpression(@"^[0-9+() .-]{8,20}$", ErrorMessage = "Số điện thoại không hợp lệ.")]
    [Display(Name = "Số điện thoại")]
    public string? SoDienThoai { get; set; }

    [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
    [StringLength(254)]
    public string? Email { get; set; }

    [StringLength(500)]
    [Display(Name = "Địa chỉ")]
    public string? DiaChi { get; set; }

    [StringLength(50)]
    [Display(Name = "Mã số thuế")]
    public string? MaSoThue { get; set; }

    public bool TrangThai { get; set; } = true;
}

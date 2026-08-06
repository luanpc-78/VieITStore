using System.ComponentModel.DataAnnotations;
using VieITStore.Models.Entities;

namespace VieITStore.Models.ViewModels;

public class BanHangTaiQuayVM
{
    [Required(ErrorMessage = "Vui lòng chọn khách hàng")]
    [Range(1, int.MaxValue, ErrorMessage = "Khách hàng không hợp lệ")]
    public int KhachHangId { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn phương thức thanh toán")]
    public PhuongThucThanhToan PhuongThucThanhToan { get; set; } = PhuongThucThanhToan.TienMat;

    public bool DaThanhToan { get; set; } = true;

    [StringLength(500, ErrorMessage = "Ghi chú không được vượt quá 500 ký tự")]
    public string? GhiChu { get; set; }

    [Required]
    public string RequestToken { get; set; } = string.Empty;

    [MinLength(1, ErrorMessage = "Đơn hàng phải có ít nhất một sản phẩm")]
    public List<BanHangTaiQuayItemVM> ChiTiets { get; set; } = [];
}

public class BanHangTaiQuayItemVM
{
    [Range(1, int.MaxValue, ErrorMessage = "Sản phẩm không hợp lệ")]
    public int SanPhamId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
    public int SoLuong { get; set; }
}

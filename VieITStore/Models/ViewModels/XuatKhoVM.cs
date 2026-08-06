using System.ComponentModel.DataAnnotations;
using VieITStore.Models.Entities;

namespace VieITStore.Models.ViewModels;

public class TaoPhieuDieuChinhTonKhoVM
{
    [Required(ErrorMessage = "Vui lòng chọn loại giao dịch")]
    public LoaiGiaoDichTonKho LoaiGiaoDich { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập lý do")]
    [StringLength(500, ErrorMessage = "Lý do không được vượt quá 500 ký tự")]
    public string LyDo { get; set; } = string.Empty;

    [MinLength(1, ErrorMessage = "Vui lòng thêm ít nhất một sản phẩm")]
    public List<TaoChiTietDieuChinhTonKhoVM> ChiTiets { get; set; } = [];
}

public class TaoChiTietDieuChinhTonKhoVM
{
    [Range(1, int.MaxValue, ErrorMessage = "Sản phẩm không hợp lệ")]
    public int SanPhamId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Số lượng thay đổi phải lớn hơn 0")]
    public int SoLuongThayDoi { get; set; }
}

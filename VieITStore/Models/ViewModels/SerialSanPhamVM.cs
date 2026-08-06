using System.ComponentModel.DataAnnotations;
using VieITStore.Models.Entities;

namespace VieITStore.Models.ViewModels;

public class SerialSanPhamVM
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập serial/IMEI.")]
    [StringLength(150, ErrorMessage = "Serial/IMEI không được vượt quá 150 ký tự.")]
    [Display(Name = "Serial/IMEI")]
    public string MaSerial { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn sản phẩm.")]
    [Display(Name = "Sản phẩm")]
    public int SanPhamId { get; set; }

    [Display(Name = "Nguồn nhập")]
    public int? ChiTietPhieuNhapId { get; set; }

    [Display(Name = "Chi tiết đơn bán")]
    public int? ChiTietDonHangId { get; set; }

    [Display(Name = "Trạng thái")]
    public TrangThaiSerial TrangThai { get; set; } = TrangThaiSerial.TrongKho;

    [StringLength(500)]
    [Display(Name = "Ghi chú")]
    public string? GhiChu { get; set; }
}

using System.ComponentModel.DataAnnotations;

namespace VieITStore.Models.Entities;

public enum LoaiGiaoDichTonKho
{
    [Display(Name = "Xuất nội bộ")]
    XuatNoiBo = 1,

    [Display(Name = "Hàng lỗi")]
    HangLoi = 2,

    [Display(Name = "Điều chỉnh tăng")]
    DieuChinhTang = 3,

    [Display(Name = "Điều chỉnh giảm")]
    DieuChinhGiam = 4
}

public class PhieuDieuChinhTonKho
{
    public int Id { get; set; }

    [Required, MaxLength(40)]
    public string MaPhieu { get; set; } = string.Empty;

    public DateTime NgayTao { get; set; } = DateTime.Now;

    public int NguoiThucHienId { get; set; }
    public NguoiDung? NguoiThucHien { get; set; }

    public LoaiGiaoDichTonKho LoaiGiaoDich { get; set; }

    [Required, MaxLength(500)]
    public string LyDo { get; set; } = string.Empty;

    public ICollection<ChiTietDieuChinhTonKho> ChiTiets { get; set; } = new List<ChiTietDieuChinhTonKho>();
}

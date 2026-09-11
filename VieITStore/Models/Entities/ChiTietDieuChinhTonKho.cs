namespace VieITStore.Models.Entities;

public class ChiTietDieuChinhTonKho
{
    public int Id { get; set; }

    public int PhieuDieuChinhTonKhoId { get; set; }
    public PhieuDieuChinhTonKho? PhieuDieuChinhTonKho { get; set; }

    public int SanPhamId { get; set; }
    public SanPham? SanPham { get; set; }

    public int SoLuongTruoc { get; set; }
    public int SoLuongThayDoi { get; set; }
    public int SoLuongSau { get; set; }
}

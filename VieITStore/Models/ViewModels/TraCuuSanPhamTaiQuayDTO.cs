namespace VieITStore.Models.ViewModels;

public sealed class TraCuuSanPhamTaiQuayDTO
{
    public int Id { get; init; }
    public string MaSanPham { get; init; } = string.Empty;
    public string? MaVach { get; init; }
    public string TenSanPham { get; init; } = string.Empty;
    public int SoLuongTon { get; init; }
    public decimal DonGia { get; init; }
}

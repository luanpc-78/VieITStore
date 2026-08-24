using VieITStore.Models.Entities;

namespace VieITStore.Models.ViewModels;

public class HomeVM
{
    public List<SanPham> SanPhamNoiBat { get; set; } = [];
    public List<SanPham> SanPhamMoi { get; set; } = [];
    public List<SanPham> SanPhamDeXuat { get; set; } = [];
    public List<DanhMuc> DanhMucs { get; set; } = [];
}

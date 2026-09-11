namespace VieITStore.Models.ViewModels
{
    public class GioHangVM
    {
        public List<GioHangItem> Items { get; set; } = new List<GioHangItem>();
        public decimal TongTien => Items.Sum(i => i.ThanhTien);
        public int TongSoLuong => Items.Sum(i => i.SoLuong);
        public bool CoTheThanhToan => Items.Count > 0 && Items.All(i => i.HopLe);
    }

    public class GioHangItem
    {
        public int SanPhamId { get; set; }
        public string MaSanPham { get; set; } = string.Empty;
        public string TenSanPham { get; set; } = string.Empty;
        public string? HinhAnh { get; set; }
        public decimal DonGia { get; set; }
        public int SoLuong { get; set; }
        public int SoLuongTon { get; set; }
        public bool DangBan { get; set; }
        public bool HopLe => DangBan && SoLuongTon > 0 && SoLuong > 0 && SoLuong <= SoLuongTon;
        public string? CanhBao => !DangBan
            ? "Sản phẩm đã ngừng bán."
            : SoLuongTon <= 0
                ? "Sản phẩm đã hết hàng."
                : SoLuong > SoLuongTon
                    ? $"Chỉ còn {SoLuongTon} sản phẩm."
                    : null;
        public decimal ThanhTien => DonGia * SoLuong;
    }
}

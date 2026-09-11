using System.ComponentModel.DataAnnotations;
using VieITStore.Models.Entities;

namespace VieITStore.Models.ViewModels
{
    public class ThanhToanVM
    {
        public GioHangVM GioHang { get; set; } = new GioHangVM();
        public List<DiaChiGiaoHang> DiaChiGiaoHangs { get; set; } = new List<DiaChiGiaoHang>();

        public int? DiaChiGiaoHangId { get; set; }

        public HinhThucNhanHang HinhThucNhanHang { get; set; } = HinhThucNhanHang.GiaoTanNoi;

        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        public string HoTenNguoiNhan { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập SĐT")]
        public string SoDienThoai { get; set; } = string.Empty;

        public string TinhThanh { get; set; } = string.Empty;

        public string QuanHuyen { get; set; } = string.Empty;

        public string PhuongXa { get; set; } = string.Empty;

        public string DiaChiChiTiet { get; set; } = string.Empty;

        public PhuongThucThanhToan PhuongThucThanhToan { get; set; } = PhuongThucThanhToan.COD;
        public string? GhiChu { get; set; }
        public bool LuuDiaChi { get; set; } = true;
    }
}

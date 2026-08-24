using VieITStore.Models.Entities;
namespace VieITStore.Models.ViewModels;
public class KhachHangAdminItemVM { public int Id { get; set; } public int NguoiDungId { get; set; } public string HoTen { get; set; }=""; public string Email { get; set; }=""; public string? SoDienThoai { get; set; } public bool TrangThai { get; set; } public DateTime NgayTao { get; set; } public int SoDon { get; set; } public decimal TongChiTieu { get; set; } }
public class KhachHangAdminDetailsVM { public KhachHang KhachHang { get; set; }=null!; public List<DonHang> DonHangs { get; set; }=[]; }

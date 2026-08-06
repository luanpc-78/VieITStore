using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VieITStore.Models;
using VieITStore.Models.Entities;
using VieITStore.Filters;

namespace VieITStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AuthorizeRole(VaiTro.Admin, VaiTro.NhanVien)]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(ApplicationDbContext context, ILogger<DashboardController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var today = DateTime.Today;
                var thisMonth = new DateTime(today.Year, today.Month, 1);

                var dashboard = new
                {
                    TongDonHangHomNay = await _context.DonHangs.CountAsync(d => d.NgayDat.Date == today),
                    DoanhThuHomNay = await _context.DonHangs
                        .Where(d => d.NgayDat.Date == today && d.TrangThai != TrangThaiDonHang.DaHuy)
                        .SumAsync(d => d.TongThanhToan),
                    DoanhThuThang = await _context.DonHangs
                        .Where(d => d.NgayDat >= thisMonth && d.TrangThai != TrangThaiDonHang.DaHuy)
                        .SumAsync(d => d.TongThanhToan),
                    DonHangChoXacNhan = await _context.DonHangs.CountAsync(d => d.TrangThai == TrangThaiDonHang.ChoXacNhan),
                    TongSanPham = await _context.SanPhams.CountAsync(),
                    SanPhamSapHet = await _context.SanPhams.CountAsync(s => s.SoLuongTon <= 10),
                    TongKhachHang = await _context.KhachHangs.CountAsync()
                };

                ViewBag.DonHangMoi = await _context.DonHangs
                    .Include(d => d.KhachHang)
                    .ThenInclude(k => k!.NguoiDung)
                    .OrderByDescending(d => d.NgayDat)
                    .Take(5)
                    .ToListAsync();

                return View(dashboard);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Dashboard Index");
                return StatusCode(500, "Có lỗi xảy ra");
            }
        }
    }
}
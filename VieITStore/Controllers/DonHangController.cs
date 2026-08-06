using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VieITStore.Models;
using VieITStore.Models.Entities;

namespace VieITStore.Controllers
{
    public class DonHangController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DonHangController> _logger;

        public DonHangController(ApplicationDbContext context, ILogger<DonHangController> logger)
        {
            _context = context;
            _logger = logger;
        }

        private int? GetUserId() => HttpContext.Session.GetInt32("UserId");

        public async Task<IActionResult> Index()
        {
            try
            {
                var userId = GetUserId();
                if (!userId.HasValue) return RedirectToAction("DangNhap", "TaiKhoan");

                var khachHang = await _context.KhachHangs
                    .FirstOrDefaultAsync(k => k.NguoiDungId == userId);

                if (khachHang == null) return NotFound();

                var donHangs = await _context.DonHangs
                    .Include(d => d.ChiTietDonHangs)
                    .ThenInclude(ct => ct.SanPham)
                    .Where(d => d.KhachHangId == khachHang.Id)
                    .OrderByDescending(d => d.NgayDat)
                    .ToListAsync();

                return View(donHangs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DonHang Index");
                return StatusCode(500, "Có lỗi xảy ra");
            }
        }

        public async Task<IActionResult> ChiTiet(int? id)
        {
            try
            {
                if (id == null) return NotFound();

                var userId = GetUserId();
                var khachHang = await _context.KhachHangs
                    .FirstOrDefaultAsync(k => k.NguoiDungId == userId);

                var donHang = await _context.DonHangs
                    .Include(d => d.ChiTietDonHangs)
                    .ThenInclude(ct => ct.SanPham)
                    .FirstOrDefaultAsync(d => d.Id == id);

                if (donHang == null) return NotFound();
                if (khachHang != null && donHang.KhachHangId != khachHang.Id) return Forbid();

                return View(donHang);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DonHang ChiTiet");
                return StatusCode(500, "Có lỗi xảy ra");
            }
        }

        [HttpPost]
        public async Task<IActionResult> HuyDon(int id, string lyDo)
        {
            try
            {
                var userId = GetUserId();
                var khachHang = await _context.KhachHangs
                    .FirstOrDefaultAsync(k => k.NguoiDungId == userId);

                var donHang = await _context.DonHangs
                    .Include(d => d.ChiTietDonHangs)
                    .ThenInclude(ct => ct.SanPham)
                    .FirstOrDefaultAsync(d => d.Id == id);

                if (donHang == null || donHang.KhachHangId != khachHang?.Id) return NotFound();

                // Chỉ hủy khi đơn chưa giao
                if (donHang.TrangThai == TrangThaiDonHang.ChoXacNhan ||
                    donHang.TrangThai == TrangThaiDonHang.DaXacNhan)
                {
                    donHang.TrangThai = TrangThaiDonHang.DaHuy;
                    donHang.LyDoHuy = lyDo;

                    // Hoàn trả tồn kho
                    foreach (var ct in donHang.ChiTietDonHangs)
                    {
                        ct.SanPham!.SoLuongTon += ct.SoLuong;
                        ct.SanPham.SoLuongDaBan -= ct.SoLuong;
                        ct.SanPham.NgayCapNhat = DateTime.Now;
                    }

                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Đã hủy đơn hàng thành công!";
                }
                else
                {
                    TempData["Error"] = "Không thể hủy đơn hàng ở trạng thái hiện tại!";
                }

                return RedirectToAction("ChiTiet", new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in HuyDon");
                TempData["Error"] = $"Lỗi: {ex.Message}";
                return RedirectToAction("ChiTiet", new { id });
            }
        }
    }
}
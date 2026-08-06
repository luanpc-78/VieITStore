using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VieITStore.Models;
using VieITStore.Models.Entities;
using VieITStore.Filters;

namespace VieITStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AuthorizeRole(VaiTro.Admin, VaiTro.NhanVien)]
    public class NhapHangController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<NhapHangController> _logger;

        public NhapHangController(ApplicationDbContext context, ILogger<NhapHangController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var phieuNhaps = await _context.PhieuNhaps
                    .Include(p => p.NhaCungCap)
                    .Include(p => p.NguoiNhap)
                    .OrderByDescending(p => p.NgayNhap)
                    .ToListAsync();

                return View(phieuNhaps);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in NhapHang Index");
                return StatusCode(500, "Có lỗi xảy ra");
            }
        }

        public async Task<IActionResult> Create()
        {
            try
            {
                ViewBag.NhaCungCaps = await _context.NhaCungCaps.Where(n => n.TrangThai).ToListAsync();
                ViewBag.SanPhams = await _context.SanPhams.Where(s => s.TrangThai).ToListAsync();
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Create GET");
                return StatusCode(500, "Có lỗi xảy ra");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PhieuNhap phieuNhap, List<ChiTietPhieuNhap> chiTiets)
        {
            try
            {
                var nguoiNhapId = HttpContext.Session.GetInt32("UserId");
                if (nguoiNhapId == null) return RedirectToAction("DangNhap", "TaiKhoan", new { area = "" });

                if (!await _context.NhaCungCaps.AnyAsync(x => x.Id == phieuNhap.NhaCungCapId && x.TrangThai))
                    ModelState.AddModelError(nameof(phieuNhap.NhaCungCapId), "Nhà cung cấp không tồn tại hoặc đã ngừng hợp tác.");

                if (chiTiets == null || chiTiets.Count == 0)
                {
                    ModelState.AddModelError("", "Vui lòng thêm ít nhất một sản phẩm");
                }

                if (!ModelState.IsValid)
                {
                    ViewBag.NhaCungCaps = await _context.NhaCungCaps.Where(n => n.TrangThai).ToListAsync();
                    ViewBag.SanPhams = await _context.SanPhams.Where(s => s.TrangThai).ToListAsync();
                    return View(phieuNhap);
                }

                var chiTietHopLe = chiTiets!;

                phieuNhap.MaPhieuNhap = $"PN{DateTime.Now:yyyyMMdd}{new Random().Next(1000, 9999)}";
                phieuNhap.NguoiNhapId = nguoiNhapId.Value;
                phieuNhap.TongTien = chiTietHopLe.Sum(ct => ct.DonGiaNhap * ct.SoLuong);
                phieuNhap.NgayNhap = DateTime.Now;

                _context.PhieuNhaps.Add(phieuNhap);

                foreach (var ct in chiTietHopLe)
                {
                    ct.PhieuNhapId = phieuNhap.Id;
                    _context.ChiTietPhieuNhaps.Add(ct);

                    var sp = await _context.SanPhams.FindAsync(ct.SanPhamId);
                    if (sp != null)
                    {
                        sp.SoLuongTon += ct.SoLuong;
                        sp.GiaNhap = ct.DonGiaNhap;
                        sp.NgayCapNhat = DateTime.Now;
                    }
                }

                await _context.SaveChangesAsync();
                TempData["Success"] = "Nhập hàng thành công!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Create POST");
                ModelState.AddModelError("", $"Lỗi: {ex.Message}");
                ViewBag.NhaCungCaps = await _context.NhaCungCaps.Where(n => n.TrangThai).ToListAsync();
                ViewBag.SanPhams = await _context.SanPhams.Where(s => s.TrangThai).ToListAsync();
                return View();
            }
        }
    }
}

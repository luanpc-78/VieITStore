using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VieITStore.Models;
using VieITStore.Models.Entities;
using VieITStore.Models.ViewModels;

namespace VieITStore.Controllers
{
    public class TaiKhoanController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TaiKhoanController> _logger;

        public TaiKhoanController(ApplicationDbContext context, ILogger<TaiKhoanController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult DangNhap(string? returnUrl = null)
        {
            if (HttpContext.Session.GetInt32("UserId").HasValue)
                return RedirectToAction("Index", "Home");
            try
            {
                ViewBag.ReturnUrl = returnUrl;
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DangNhap GET");
                return StatusCode(500, "Có lỗi xảy ra");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DangNhap(LoginVM model, string? returnUrl = null)
        {
            try
            {
                if (!ModelState.IsValid) return View(model);

                var normalizedEmail = model.Email.Trim().ToLowerInvariant();
                var user = await _context.NguoiDungs
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == normalizedEmail && u.TrangThai);

                if (user == null)
                {
                    ModelState.AddModelError("", "Email hoặc mật khẩu không đúng.");
                    return View(model);
                }

                // Verify password with BCrypt
                bool passwordValid;
                try
                {
                    passwordValid = BCrypt.Net.BCrypt.Verify(model.MatKhau, user.MatKhauHash);
                }
                catch (BCrypt.Net.SaltParseException)
                {
                    // Nâng cấp dữ liệu demo cũ đang lưu plain text sang BCrypt ngay lần đăng nhập đầu.
                    passwordValid = user.MatKhauHash == model.MatKhau;
                    if (passwordValid) user.MatKhauHash = BCrypt.Net.BCrypt.HashPassword(model.MatKhau);
                }

                if (!passwordValid)
                {
                    ModelState.AddModelError("", "Email hoặc mật khẩu không đúng.");
                    return View(model);
                }

                HttpContext.Session.SetInt32("UserId", user.Id);
                HttpContext.Session.SetString("Email", user.Email);
                HttpContext.Session.SetString("HoTen", user.HoTen);
                HttpContext.Session.SetString("VaiTro", user.VaiTro.ToString());

                await MergeSessionCart(user.Id, HttpContext.Session.Id);
                user.LanDangNhapCuoi = DateTime.Now;
                await _context.SaveChangesAsync();

                return user.VaiTro switch
                {
                    VaiTro.Admin => RedirectToAction("Index", "Dashboard", new { area = "Admin" }),
                    VaiTro.NhanVien => RedirectToAction("Index", "Dashboard", new { area = "Admin" }),
                    _ => RedirectToAction("Index", "Home")
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DangNhap POST");
                ModelState.AddModelError("", "Không thể đăng nhập lúc này. Vui lòng thử lại.");
                return View(model);
            }
        }

        private async Task MergeSessionCart(int userId, string sessionId)
        {
            var sessionItems = await _context.GioHangs
                .Include(x => x.SanPham)
                .Where(x => x.NguoiDungId == null && x.SessionId == sessionId)
                .ToListAsync();
            if (sessionItems.Count == 0) return;

            var productIds = sessionItems.Select(x => x.SanPhamId).Distinct().ToList();
            var userItems = await _context.GioHangs
                .Where(x => x.NguoiDungId == userId && productIds.Contains(x.SanPhamId))
                .ToDictionaryAsync(x => x.SanPhamId);

            foreach (var sessionItem in sessionItems)
            {
                var stock = Math.Max(0, sessionItem.SanPham?.SoLuongTon ?? 0);
                if (stock == 0)
                {
                    _context.GioHangs.Remove(sessionItem);
                    continue;
                }

                if (userItems.TryGetValue(sessionItem.SanPhamId, out var userItem))
                {
                    var combinedQuantity = (long)userItem.SoLuong + sessionItem.SoLuong;
                    userItem.SoLuong = (int)Math.Min(stock, combinedQuantity);
                    _context.GioHangs.Remove(sessionItem);
                }
                else
                {
                    sessionItem.SoLuong = Math.Min(stock, sessionItem.SoLuong);
                    sessionItem.NguoiDungId = userId;
                    sessionItem.SessionId = null;
                    userItems.Add(sessionItem.SanPhamId, sessionItem);
                }
            }
        }

        [HttpGet]
        public IActionResult DangKy()
        {
            if (HttpContext.Session.GetInt32("UserId").HasValue)
                return RedirectToAction("Index", "Home");
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DangKy GET");
                return StatusCode(500, "Có lỗi xảy ra");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DangKy(RegisterVM model)
        {
            try
            {
                if (!ModelState.IsValid) return View(model);

                var normalizedEmail = model.Email.Trim().ToLowerInvariant();
                if (await _context.NguoiDungs.AnyAsync(u => u.Email.ToLower() == normalizedEmail))
                {
                    ModelState.AddModelError("Email", "Email đã được sử dụng!");
                    return View(model);
                }

                var user = new NguoiDung
                {
                    // Cột cũ được đồng bộ nội bộ để tương thích dữ liệu/migration lịch sử.
                    TenDangNhap = normalizedEmail,
                    MatKhauHash = BCrypt.Net.BCrypt.HashPassword(model.MatKhau),
                    HoTen = model.HoTen.Trim(),
                    Email = normalizedEmail,
                    SoDienThoai = model.SoDienThoai.Trim().Replace(" ", string.Empty),
                    VaiTro = VaiTro.KhachHang
                };

                _context.NguoiDungs.Add(user);
                await _context.SaveChangesAsync();

                var khachHang = new KhachHang
                {
                    NguoiDungId = user.Id
                };
                _context.KhachHangs.Add(khachHang);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Đăng ký thành công! Vui lòng đăng nhập.";
                return RedirectToAction("DangNhap");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DangKy POST");
                ModelState.AddModelError("", "Không thể đăng ký lúc này. Vui lòng thử lại.");
                return View(model);
            }
        }

        public IActionResult DangXuat()
        {
            try
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DangXuat");
                return RedirectToAction("Index", "Home");
            }
        }

    }
}

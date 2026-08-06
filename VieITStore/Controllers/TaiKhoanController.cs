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

                var user = await _context.NguoiDungs
                    .FirstOrDefaultAsync(u => u.TenDangNhap == model.TenDangNhap && u.TrangThai);

                if (user == null)
                {
                    ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng!");
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
                    ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng!");
                    return View(model);
                }

                HttpContext.Session.SetInt32("UserId", user.Id);
                HttpContext.Session.SetString("UserName", user.HoTen);
                HttpContext.Session.SetString("VaiTro", user.VaiTro.ToString());

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
                ModelState.AddModelError("", "Có lỗi xảy ra: " + ex.Message);
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult DangKy()
        {
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

                if (await _context.NguoiDungs.AnyAsync(u => u.TenDangNhap == model.TenDangNhap))
                {
                    ModelState.AddModelError("TenDangNhap", "Tên đăng nhập đã tồn tại!");
                    return View(model);
                }

                if (await _context.NguoiDungs.AnyAsync(u => u.Email == model.Email))
                {
                    ModelState.AddModelError("Email", "Email đã được sử dụng!");
                    return View(model);
                }

                var user = new NguoiDung
                {
                    TenDangNhap = model.TenDangNhap,
                    MatKhauHash = BCrypt.Net.BCrypt.HashPassword(model.MatKhau),
                    HoTen = model.HoTen,
                    Email = model.Email,
                    SoDienThoai = model.SoDienThoai,
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
                ModelState.AddModelError("", "Có lỗi xảy ra: " + ex.Message);
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

using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VieITStore.Models;
using VieITStore.Models.ViewModels;

namespace VieITStore.Controllers;

public class KhoiPhucMatKhauController : Controller
{
    private const string ResetUserKey = "ResetUser";
    private const string ResetCodeKey = "ResetCode";
    private const string ResetExpiresKey = "ResetExpires";
    private readonly ApplicationDbContext _context;

    public KhoiPhucMatKhauController(ApplicationDbContext context) => _context = context;

    [HttpGet]
    public IActionResult QuenMatKhau() => View(new QuenMatKhauVM());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> QuenMatKhau(QuenMatKhauVM model)
    {
        if (!ModelState.IsValid) return View(model);

        ClearResetSession();
        var user = await _context.NguoiDungs
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.TenDangNhap == model.TaiKhoan || x.Email == model.TaiKhoan);

        if (user != null)
        {
            var code = RandomNumberGenerator.GetInt32(100000, 1000000).ToString();
            HttpContext.Session.SetString(ResetUserKey, user.TenDangNhap);
            HttpContext.Session.SetString(ResetCodeKey, code);
            HttpContext.Session.SetString(
                ResetExpiresKey,
                DateTimeOffset.UtcNow.AddMinutes(10).ToUnixTimeSeconds().ToString());
            TempData["DemoResetCode"] = code;
        }

        TempData["Info"] = "Nếu tài khoản tồn tại, mã xác nhận đã được tạo và có hiệu lực 10 phút.";
        return RedirectToAction(nameof(DatLaiMatKhau), new { taiKhoan = model.TaiKhoan });
    }

    [HttpGet]
    public IActionResult DatLaiMatKhau(string? taiKhoan) => View(new DatLaiMatKhauVM
    {
        TaiKhoan = taiKhoan ?? string.Empty
    });

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DatLaiMatKhau(DatLaiMatKhauVM model)
    {
        if (!ModelState.IsValid) return View(model);

        var resetUser = HttpContext.Session.GetString(ResetUserKey);
        var resetCode = HttpContext.Session.GetString(ResetCodeKey);
        var hasValidExpiry = long.TryParse(HttpContext.Session.GetString(ResetExpiresKey), out var expires)
            && DateTimeOffset.UtcNow.ToUnixTimeSeconds() <= expires;

        if (!hasValidExpiry || resetUser == null || resetCode != model.MaXacNhan)
        {
            ModelState.AddModelError(nameof(model.MaXacNhan), "Mã không đúng hoặc đã hết hạn.");
            return View(model);
        }

        var user = await _context.NguoiDungs.FirstOrDefaultAsync(x => x.TenDangNhap == resetUser);
        if (user == null)
        {
            ClearResetSession();
            return NotFound();
        }

        user.MatKhauHash = BCrypt.Net.BCrypt.HashPassword(model.MatKhauMoi);
        await _context.SaveChangesAsync();
        ClearResetSession();

        TempData["Success"] = "Đặt lại mật khẩu thành công.";
        return RedirectToAction("DangNhap", "TaiKhoan");
    }

    private void ClearResetSession()
    {
        HttpContext.Session.Remove(ResetUserKey);
        HttpContext.Session.Remove(ResetCodeKey);
        HttpContext.Session.Remove(ResetExpiresKey);
    }
}

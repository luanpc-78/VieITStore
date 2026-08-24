using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VieITStore.Models;
using VieITStore.Models.ViewModels;

namespace VieITStore.Controllers;

public class KhoiPhucMatKhauController : Controller
{
    private const string ResetEmailKey = "ResetEmail";
    private const string ResetCodeKey = "ResetCode";
    private const string ResetExpiresKey = "ResetExpires";
    private readonly ApplicationDbContext _context;

    public KhoiPhucMatKhauController(ApplicationDbContext context) => _context = context;

    [HttpGet]
    public IActionResult QuenMatKhau() => View(new QuenMatKhauVM());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> QuenMatKhau(QuenMatKhauVM model)
    {
        if (!ModelState.IsValid) return View(model);
        ClearResetSession();
        var email = model.Email.Trim().ToLowerInvariant();
        var user = await _context.NguoiDungs.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Email.ToLower() == email && x.TrangThai);

        if (user != null)
        {
            var code = RandomNumberGenerator.GetInt32(100000, 1000000).ToString();
            HttpContext.Session.SetString(ResetEmailKey, user.Email);
            HttpContext.Session.SetString(ResetCodeKey, code);
            HttpContext.Session.SetString(ResetExpiresKey,
                DateTimeOffset.UtcNow.AddMinutes(10).ToUnixTimeSeconds().ToString());
            TempData["DemoResetCode"] = code;
        }

        TempData["Info"] = "Nếu email tồn tại, mã xác nhận đã được tạo và có hiệu lực 10 phút.";
        return RedirectToAction(nameof(DatLaiMatKhau), new { email });
    }

    [HttpGet]
    public IActionResult DatLaiMatKhau(string? email) => View(new DatLaiMatKhauVM { Email = email ?? string.Empty });

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> DatLaiMatKhau(DatLaiMatKhauVM model)
    {
        if (!ModelState.IsValid) return View(model);
        var resetEmail = HttpContext.Session.GetString(ResetEmailKey);
        var resetCode = HttpContext.Session.GetString(ResetCodeKey);
        var validExpiry = long.TryParse(HttpContext.Session.GetString(ResetExpiresKey), out var expires)
            && DateTimeOffset.UtcNow.ToUnixTimeSeconds() <= expires;

        if (!validExpiry || resetEmail == null || resetCode != model.MaXacNhan)
        {
            ModelState.AddModelError(nameof(model.MaXacNhan), "Mã không đúng hoặc đã hết hạn.");
            return View(model);
        }

        var user = await _context.NguoiDungs.FirstOrDefaultAsync(x => x.Email == resetEmail);
        if (user == null) { ClearResetSession(); return NotFound(); }
        user.MatKhauHash = BCrypt.Net.BCrypt.HashPassword(model.MatKhauMoi);
        await _context.SaveChangesAsync();
        ClearResetSession();
        TempData["Success"] = "Đặt lại mật khẩu thành công.";
        return RedirectToAction("DangNhap", "TaiKhoan");
    }

    private void ClearResetSession()
    {
        HttpContext.Session.Remove(ResetEmailKey);
        HttpContext.Session.Remove(ResetCodeKey);
        HttpContext.Session.Remove(ResetExpiresKey);
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VieITStore.Filters;
using VieITStore.Models;
using VieITStore.Models.Entities;
using VieITStore.Models.ViewModels;

namespace VieITStore.Controllers;

[AuthorizeRole(VaiTro.KhachHang)]
public class HoSoController : Controller
{
    private readonly ApplicationDbContext _context;

    public HoSoController(ApplicationDbContext context) => _context = context;

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue) return RedirectToLogin();

        var user = await _context.NguoiDungs
            .AsNoTracking()
            .Include(x => x.KhachHang)
            .ThenInclude(x => x!.DiaChiGiaoHangs)
            .SingleOrDefaultAsync(x => x.Id == userId.Value);
        if (user?.KhachHang == null) return NotFound();

        return View(new HoSoVM
        {
            HoTen = user.HoTen,
            Email = user.Email ?? string.Empty,
            SoDienThoai = user.SoDienThoai ?? string.Empty,
            GioiTinh = user.KhachHang.GioiTinh,
            NgaySinh = user.KhachHang.NgaySinh,
            DiaChis = user.KhachHang.DiaChiGiaoHangs
                .OrderByDescending(x => x.LaMacDinh)
                .ThenBy(x => x.Id)
                .ToList()
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(HoSoVM model)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue) return RedirectToLogin();

        if (await _context.NguoiDungs.AnyAsync(x => x.Id != userId.Value && x.Email == model.Email))
            ModelState.AddModelError(nameof(model.Email), "Email đã được sử dụng.");

        if (!ModelState.IsValid)
        {
            model.DiaChis = await GetOwnAddresses(userId.Value);
            return View(model);
        }

        var user = await _context.NguoiDungs
            .Include(x => x.KhachHang)
            .SingleOrDefaultAsync(x => x.Id == userId.Value);
        if (user?.KhachHang == null) return NotFound();

        user.HoTen = model.HoTen.Trim();
        user.Email = model.Email.Trim();
        user.SoDienThoai = model.SoDienThoai.Trim();
        user.KhachHang.GioiTinh = model.GioiTinh;
        user.KhachHang.NgaySinh = model.NgaySinh;

        await _context.SaveChangesAsync();
        HttpContext.Session.SetString("UserName", user.HoTen);
        TempData["Success"] = "Đã cập nhật hồ sơ.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ThemDiaChi(
        [Bind("HoTenNguoiNhan,SoDienThoai,TinhThanh,QuanHuyen,PhuongXa,DiaChiChiTiet,LaMacDinh,GhiChu")]
        DiaChiGiaoHang model)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue) return RedirectToLogin();

        var customer = await _context.KhachHangs
            .Include(x => x.DiaChiGiaoHangs)
            .SingleOrDefaultAsync(x => x.NguoiDungId == userId.Value);
        if (customer == null) return NotFound();

        ModelState.Remove(nameof(model.KhachHangId));
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Thông tin địa chỉ chưa hợp lệ.";
            return RedirectToAction(nameof(Index));
        }

        var isFirstAddress = customer.DiaChiGiaoHangs.Count == 0;
        if (model.LaMacDinh || isFirstAddress)
            foreach (var address in customer.DiaChiGiaoHangs) address.LaMacDinh = false;

        model.KhachHangId = customer.Id;
        model.LaMacDinh = model.LaMacDinh || isFirstAddress;
        _context.DiaChiGiaoHangs.Add(model);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Đã thêm địa chỉ nhận hàng.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> XoaDiaChi(int id)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue) return RedirectToLogin();

        var address = await _context.DiaChiGiaoHangs
            .Include(x => x.KhachHang)
            .SingleOrDefaultAsync(x => x.Id == id && x.KhachHang!.NguoiDungId == userId.Value);
        if (address == null) return NotFound();

        var wasDefault = address.LaMacDinh;
        var customerId = address.KhachHangId;
        _context.DiaChiGiaoHangs.Remove(address);

        if (wasDefault)
        {
            var replacement = await _context.DiaChiGiaoHangs
                .Where(x => x.KhachHangId == customerId && x.Id != id)
                .OrderBy(x => x.Id)
                .FirstOrDefaultAsync();
            if (replacement != null) replacement.LaMacDinh = true;
        }

        await _context.SaveChangesAsync();
        TempData["Success"] = "Đã xóa địa chỉ nhận hàng.";
        return RedirectToAction(nameof(Index));
    }

    private Task<List<DiaChiGiaoHang>> GetOwnAddresses(int userId) => _context.DiaChiGiaoHangs
        .AsNoTracking()
        .Where(x => x.KhachHang!.NguoiDungId == userId)
        .OrderByDescending(x => x.LaMacDinh)
        .ThenBy(x => x.Id)
        .ToListAsync();

    private RedirectToActionResult RedirectToLogin() => RedirectToAction(
        "DangNhap", "TaiKhoan", new { returnUrl = Url.Action(nameof(Index), "HoSo") });
}

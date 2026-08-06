using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VieITStore.Filters;
using VieITStore.Models;
using VieITStore.Models.Entities;
using VieITStore.Models.ViewModels;

namespace VieITStore.Areas.Admin.Controllers;

[Area("Admin")]
[AuthorizeRole(VaiTro.Admin, VaiTro.NhanVien)]
public class NhaCungCapController : Controller
{
    private readonly ApplicationDbContext _context;

    public NhaCungCapController(ApplicationDbContext context) => _context = context;

    public async Task<IActionResult> Index(string? search)
    {
        var query = _context.NhaCungCaps.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.Trim();
            query = query.Where(x => x.MaNCC.Contains(search)
                || x.TenNCC.Contains(search)
                || (x.NguoiLienHe != null && x.NguoiLienHe.Contains(search))
                || (x.SoDienThoai != null && x.SoDienThoai.Contains(search)));
        }

        ViewBag.Search = search;
        return View(await query.OrderByDescending(x => x.TrangThai).ThenBy(x => x.TenNCC).ToListAsync());
    }

    public IActionResult Create() => View(new NhaCungCapVM());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(NhaCungCapVM model)
    {
        Normalize(model);
        await ValidateUniqueCode(model.MaNCC, null);
        if (!ModelState.IsValid) return View(model);

        _context.NhaCungCaps.Add(ToEntity(model));
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError(nameof(model.MaNCC), "Mã nhà cung cấp đã tồn tại.");
            return View(model);
        }

        TempData["Success"] = "Đã thêm nhà cung cấp.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var entity = await _context.NhaCungCaps.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        return entity == null ? NotFound() : View(ToViewModel(entity));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, NhaCungCapVM model)
    {
        if (id != model.Id) return NotFound();
        Normalize(model);
        await ValidateUniqueCode(model.MaNCC, id);
        if (!ModelState.IsValid) return View(model);

        var entity = await _context.NhaCungCaps.FindAsync(id);
        if (entity == null) return NotFound();

        entity.MaNCC = model.MaNCC;
        entity.TenNCC = model.TenNCC;
        entity.NguoiLienHe = model.NguoiLienHe;
        entity.SoDienThoai = model.SoDienThoai;
        entity.Email = model.Email;
        entity.DiaChi = model.DiaChi;
        entity.MaSoThue = model.MaSoThue;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError(nameof(model.MaNCC), "Mã nhà cung cấp đã tồn tại.");
            return View(model);
        }

        TempData["Success"] = "Đã cập nhật nhà cung cấp.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details(int id)
    {
        var entity = await _context.NhaCungCaps.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null) return NotFound();

        ViewBag.SoPhieuNhap = await _context.PhieuNhaps.CountAsync(x => x.NhaCungCapId == id);
        return View(entity);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleStatus(int id)
    {
        var entity = await _context.NhaCungCaps.FindAsync(id);
        if (entity == null) return NotFound();

        entity.TrangThai = !entity.TrangThai;
        await _context.SaveChangesAsync();
        TempData["Success"] = entity.TrangThai
            ? "Đã kích hoạt lại nhà cung cấp."
            : "Đã ngừng hợp tác với nhà cung cấp.";
        return RedirectToAction(nameof(Index));
    }

    private async Task ValidateUniqueCode(string code, int? excludedId)
    {
        if (string.IsNullOrWhiteSpace(code)) return;
        if (await _context.NhaCungCaps.AnyAsync(x => x.MaNCC == code && (!excludedId.HasValue || x.Id != excludedId)))
            ModelState.AddModelError(nameof(NhaCungCapVM.MaNCC), "Mã nhà cung cấp đã tồn tại.");
    }

    private static void Normalize(NhaCungCapVM model)
    {
        model.MaNCC = model.MaNCC?.Trim().ToUpperInvariant() ?? string.Empty;
        model.TenNCC = model.TenNCC?.Trim() ?? string.Empty;
        model.NguoiLienHe = Clean(model.NguoiLienHe);
        model.SoDienThoai = Clean(model.SoDienThoai);
        model.Email = Clean(model.Email);
        model.DiaChi = Clean(model.DiaChi);
        model.MaSoThue = Clean(model.MaSoThue);
    }

    private static string? Clean(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static NhaCungCap ToEntity(NhaCungCapVM model) => new()
    {
        MaNCC = model.MaNCC,
        TenNCC = model.TenNCC,
        NguoiLienHe = model.NguoiLienHe,
        SoDienThoai = model.SoDienThoai,
        Email = model.Email,
        DiaChi = model.DiaChi,
        MaSoThue = model.MaSoThue,
        TrangThai = true
    };

    private static NhaCungCapVM ToViewModel(NhaCungCap entity) => new()
    {
        Id = entity.Id,
        MaNCC = entity.MaNCC,
        TenNCC = entity.TenNCC,
        NguoiLienHe = entity.NguoiLienHe,
        SoDienThoai = entity.SoDienThoai,
        Email = entity.Email,
        DiaChi = entity.DiaChi,
        MaSoThue = entity.MaSoThue,
        TrangThai = entity.TrangThai
    };
}

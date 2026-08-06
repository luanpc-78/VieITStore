using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VieITStore.Filters;
using VieITStore.Models;
using VieITStore.Models.Entities;

namespace VieITStore.Areas.Admin.Controllers;

[Area("Admin")]
[AuthorizeRole(VaiTro.Admin, VaiTro.NhanVien)]
public class DanhMucAdminController : Controller
{
    private readonly ApplicationDbContext _context;
    public DanhMucAdminController(ApplicationDbContext context) => _context = context;

    public async Task<IActionResult> Index() => View(await _context.DanhMucs.Include(x => x.SanPhams).OrderBy(x => x.ThuTu).ToListAsync());

    public IActionResult Create() => View(new DanhMuc());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(DanhMuc model)
    {
        if (await _context.DanhMucs.AnyAsync(x => x.TenDanhMuc == model.TenDanhMuc))
            ModelState.AddModelError(nameof(model.TenDanhMuc), "Tên danh mục đã tồn tại.");
        if (!ModelState.IsValid) return View(model);
        _context.Add(model);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Đã thêm danh mục.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var item = await _context.DanhMucs.FindAsync(id);
        return item == null ? NotFound() : View(item);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, DanhMuc model)
    {
        if (id != model.Id) return NotFound();
        if (await _context.DanhMucs.AnyAsync(x => x.Id != id && x.TenDanhMuc == model.TenDanhMuc))
            ModelState.AddModelError(nameof(model.TenDanhMuc), "Tên danh mục đã tồn tại.");
        if (!ModelState.IsValid) return View(model);
        _context.Update(model);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Đã cập nhật danh mục.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Toggle(int id)
    {
        var item = await _context.DanhMucs.FindAsync(id);
        if (item == null) return NotFound();
        item.HienThi = !item.HienThi;
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}

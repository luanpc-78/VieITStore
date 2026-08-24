using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VieITStore.Filters;
using VieITStore.Models;
using VieITStore.Models.Entities;
namespace VieITStore.Areas.Admin.Controllers;
[Area("Admin"), AuthorizeRole(VaiTro.Admin, VaiTro.NhanVien)]
public class DanhGiaAdminController : Controller
{
    private readonly ApplicationDbContext _context; public DanhGiaAdminController(ApplicationDbContext context)=>_context=context;
    [HttpGet] public async Task<IActionResult> Index(string? q, bool? hienThi) { var query=_context.DanhGias.AsNoTracking().Include(x=>x.SanPham).Include(x=>x.KhachHang).ThenInclude(x=>x!.NguoiDung).AsQueryable(); if(!string.IsNullOrWhiteSpace(q)){var k=q.Trim();query=query.Where(x=>x.SanPham!.TenSanPham.Contains(k)||x.KhachHang!.NguoiDung!.HoTen.Contains(k)||x.NoiDung.Contains(k));} if(hienThi.HasValue)query=query.Where(x=>x.HienThi==hienThi); ViewBag.Q=q;ViewBag.HienThi=hienThi;return View(await query.OrderByDescending(x=>x.NgayDanhGia).ToListAsync()); }
    [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> ChuyenTrangThai(int id){var review=await _context.DanhGias.FindAsync(id);if(review==null)return NotFound();review.HienThi=!review.HienThi;await _context.SaveChangesAsync();TempData["Success"]=review.HienThi?"Đã hiển thị đánh giá.":"Đã ẩn đánh giá.";return RedirectToAction(nameof(Index));}
}

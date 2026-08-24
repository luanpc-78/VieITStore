using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VieITStore.Filters;
using VieITStore.Models;
using VieITStore.Models.Entities;
using VieITStore.Models.ViewModels;
namespace VieITStore.Areas.Admin.Controllers;
[Area("Admin"), AuthorizeRole(VaiTro.Admin, VaiTro.NhanVien)]
public class KhachHangAdminController : Controller
{
    private readonly ApplicationDbContext _context; public KhachHangAdminController(ApplicationDbContext context)=>_context=context;
    [HttpGet] public async Task<IActionResult> Index(string? q,bool? trangThai){var query=_context.KhachHangs.AsNoTracking().Include(x=>x.NguoiDung).AsQueryable();if(!string.IsNullOrWhiteSpace(q)){var k=q.Trim();query=query.Where(x=>x.NguoiDung!.HoTen.Contains(k)||x.NguoiDung.Email.Contains(k)||(x.NguoiDung.SoDienThoai!=null&&x.NguoiDung.SoDienThoai.Contains(k)));}if(trangThai.HasValue)query=query.Where(x=>x.NguoiDung!.TrangThai==trangThai);var list=await query.Select(x=>new KhachHangAdminItemVM{Id=x.Id,NguoiDungId=x.NguoiDungId,HoTen=x.NguoiDung!.HoTen,Email=x.NguoiDung.Email,SoDienThoai=x.NguoiDung.SoDienThoai,TrangThai=x.NguoiDung.TrangThai,NgayTao=x.NguoiDung.NgayTao,SoDon=x.DonHangs.Count,TongChiTieu=x.DonHangs.Where(d=>d.TrangThai==TrangThaiDonHang.HoanThanh).Sum(d=>(decimal?)d.TongThanhToan)??0}).OrderByDescending(x=>x.NgayTao).ToListAsync();ViewBag.Q=q;ViewBag.TrangThai=trangThai;return View(list);}
    [HttpGet] public async Task<IActionResult> Details(int id){var customer=await _context.KhachHangs.AsNoTracking().Include(x=>x.NguoiDung).Include(x=>x.DiaChiGiaoHangs).SingleOrDefaultAsync(x=>x.Id==id);if(customer==null)return NotFound();var orders=await _context.DonHangs.AsNoTracking().Where(x=>x.KhachHangId==id).OrderByDescending(x=>x.NgayDat).ToListAsync();return View(new KhachHangAdminDetailsVM{KhachHang=customer,DonHangs=orders});}
    [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> ChuyenTrangThai(int id){var customer=await _context.KhachHangs.Include(x=>x.NguoiDung).SingleOrDefaultAsync(x=>x.Id==id);if(customer?.NguoiDung==null)return NotFound();if(customer.NguoiDungId==HttpContext.Session.GetInt32("UserId")){TempData["Error"]="Không thể tự khóa tài khoản đang đăng nhập.";return RedirectToAction(nameof(Index));}customer.NguoiDung.TrangThai=!customer.NguoiDung.TrangThai;await _context.SaveChangesAsync();TempData["Success"]=customer.NguoiDung.TrangThai?"Đã kích hoạt khách hàng.":"Đã khóa khách hàng.";return RedirectToAction(nameof(Index));}
}

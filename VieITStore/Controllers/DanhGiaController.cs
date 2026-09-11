using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VieITStore.Models;
using VieITStore.Models.Entities;
using VieITStore.Models.ViewModels;

namespace VieITStore.Controllers;
public class DanhGiaController : Controller
{
    private readonly ApplicationDbContext _context;
    public DanhGiaController(ApplicationDbContext context) => _context = context;
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Luu(DanhGiaFormVM model)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue) return RedirectToAction("DangNhap", "TaiKhoan", new { returnUrl = Url.Action("ChiTiet", "SanPham", new { id = model.SanPhamId }) });
        var customer = await _context.KhachHangs.SingleOrDefaultAsync(x => x.NguoiDungId == userId);
        if (customer == null) return Forbid();
        var purchased = await _context.ChiTietDonHangs.AnyAsync(x => x.SanPhamId == model.SanPhamId && x.DonHang!.KhachHangId == customer.Id && x.DonHang.TrangThai == TrangThaiDonHang.HoanThanh);
        if (!purchased) { TempData["Error"] = "Bạn chỉ có thể đánh giá sản phẩm đã mua trong đơn hoàn thành."; return RedirectToAction("ChiTiet", "SanPham", new { id = model.SanPhamId }); }
        if (!ModelState.IsValid) { TempData["Error"] = "Đánh giá cần từ 1–5 sao và nội dung từ 10 đến 1000 ký tự."; return RedirectToAction("ChiTiet", "SanPham", new { id = model.SanPhamId }); }
        var review = await _context.DanhGias.SingleOrDefaultAsync(x => x.KhachHangId == customer.Id && x.SanPhamId == model.SanPhamId);
        if (review == null) { review = new DanhGia { KhachHangId = customer.Id, SanPhamId = model.SanPhamId, NgayDanhGia = DateTime.Now }; _context.DanhGias.Add(review); }
        else review.NgayCapNhat = DateTime.Now;
        review.SoSao = model.SoSao; review.NoiDung = model.NoiDung.Trim(); review.HienThi = true;
        try { await _context.SaveChangesAsync(); TempData["Success"] = "Đã lưu đánh giá của bạn."; } catch (DbUpdateException) { TempData["Error"] = "Đánh giá cho sản phẩm này đã tồn tại."; }
        return RedirectToAction("ChiTiet", "SanPham", new { id = model.SanPhamId });
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VieITStore.Filters;
using VieITStore.Models;
using VieITStore.Models.Entities;

namespace VieITStore.Controllers;

public class HoaDonController : Controller
{
    private readonly ApplicationDbContext _context;

    public HoaDonController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue)
            return RedirectToAction("DangNhap", "TaiKhoan", new { returnUrl = Url.Action(nameof(Index)) });
        var role = GetRole();
        if (!role.HasValue)
            return RedirectToAction("DangNhap", "TaiKhoan");

        var query = _context.HoaDons
            .AsNoTracking()
            .Include(h => h.DonHang)
            .Include(h => h.KhachHang).ThenInclude(k => k!.NguoiDung)
            .OrderByDescending(h => h.NgayLap)
            .AsQueryable();

        if (role == VaiTro.KhachHang)
            query = query.Where(h => h.KhachHang!.NguoiDungId == userId.Value);

        return View(await query.ToListAsync());
    }

    [AuthorizeRole(VaiTro.Admin, VaiTro.NhanVien)]
    public async Task<IActionResult> Create(int donHangId)
    {
        var existingInvoiceId = await _context.HoaDons
            .Where(h => h.DonHangId == donHangId)
            .Select(h => (int?)h.Id)
            .SingleOrDefaultAsync();

        if (existingInvoiceId.HasValue)
            return RedirectToAction(nameof(Details), new { id = existingInvoiceId.Value });

        var order = await LoadOrder(donHangId);
        return order == null ? NotFound() : View(order);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [AuthorizeRole(VaiTro.Admin, VaiTro.NhanVien)]
    public async Task<IActionResult> CreateConfirmed(int donHangId)
    {
        var order = await LoadOrder(donHangId);
        if (order == null) return NotFound();

        var existingInvoiceId = await _context.HoaDons
            .Where(h => h.DonHangId == donHangId)
            .Select(h => (int?)h.Id)
            .SingleOrDefaultAsync();

        if (existingInvoiceId.HasValue)
        {
            TempData["Info"] = "Đơn hàng này đã có hóa đơn.";
            return RedirectToAction(nameof(Details), new { id = existingInvoiceId.Value });
        }

        if (!order.DaThanhToan)
        {
            TempData["Error"] = "Chỉ có thể lập hóa đơn sau khi đơn hàng đã thanh toán.";
            return RedirectToAction(nameof(Create), new { donHangId });
        }

        var invoice = new HoaDon
        {
            DonHangId = order.Id,
            SoHoaDon = $"HD-{DateTime.Now:yyyyMMdd}-{order.Id:D6}",
            NgayLap = DateTime.Now,
            KhachHangId = order.KhachHangId,
            NguoiLap = HttpContext.Session.GetString("UserName") ?? "Nhân viên",
            TongTienHang = order.TongTienHang,
            ThueVAT = 0,
            TienThueVAT = 0,
            GiamGia = order.GiamGia,
            TongThanhToan = order.TongThanhToan,
            ThanhTien = order.TongThanhToan,
            PhuongThucThanhToan = PaymentMethodText(order.PhuongThucThanhToan),
            TrangThai = "Hoàn thành",
            ChiTietHoaDons = order.ChiTietDonHangs.Select(item => new ChiTietHoaDon
            {
                SanPhamId = item.SanPhamId,
                DonGia = item.DonGia,
                SoLuong = item.SoLuong
            }).ToList()
        };

        _context.HoaDons.Add(invoice);
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            _context.ChangeTracker.Clear();
            var invoiceId = await _context.HoaDons
                .AsNoTracking()
                .Where(h => h.DonHangId == donHangId)
                .Select(h => (int?)h.Id)
                .SingleOrDefaultAsync();
            if (!invoiceId.HasValue) throw;
            TempData["Info"] = "Đơn hàng này đã có hóa đơn.";
            return RedirectToAction(nameof(Details), new { id = invoiceId.Value });
        }

        TempData["Success"] = $"Đã lập hóa đơn {invoice.SoHoaDon}.";
        return RedirectToAction(nameof(Details), new { id = invoice.Id });
    }

    public async Task<IActionResult> Details(int id)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue)
            return RedirectToAction("DangNhap", "TaiKhoan", new { returnUrl = Url.Action(nameof(Details), new { id }) });
        var role = GetRole();
        if (!role.HasValue)
            return RedirectToAction("DangNhap", "TaiKhoan");

        var invoice = await _context.HoaDons
            .AsNoTracking()
            .Include(h => h.DonHang)
            .Include(h => h.KhachHang).ThenInclude(k => k!.NguoiDung)
            .Include(h => h.ChiTietHoaDons).ThenInclude(c => c.SanPham)
            .SingleOrDefaultAsync(h => h.Id == id);

        if (invoice == null) return NotFound();
        if (role == VaiTro.KhachHang && invoice.KhachHang?.NguoiDungId != userId.Value)
            return Forbid();

        return View(invoice);
    }

    private Task<DonHang?> LoadOrder(int id) => _context.DonHangs
        .AsNoTracking()
        .Include(d => d.KhachHang).ThenInclude(k => k!.NguoiDung)
        .Include(d => d.ChiTietDonHangs).ThenInclude(c => c.SanPham)
        .SingleOrDefaultAsync(d => d.Id == id);

    private VaiTro? GetRole() => Enum.TryParse<VaiTro>(HttpContext.Session.GetString("VaiTro"), out var role)
        ? role
        : null;

    private static string PaymentMethodText(PhuongThucThanhToan method) => method switch
    {
        PhuongThucThanhToan.COD => "Thanh toán khi nhận hàng (COD)",
        PhuongThucThanhToan.ChuyenKhoan => "Chuyển khoản",
        PhuongThucThanhToan.ViDienTu => "Ví điện tử",
        PhuongThucThanhToan.TheTinDung => "Thẻ tín dụng",
        _ => method.ToString()
    };
}

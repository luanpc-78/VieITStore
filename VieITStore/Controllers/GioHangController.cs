using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VieITStore.Models;
using VieITStore.Models.Entities;
using VieITStore.Models.ViewModels;

namespace VieITStore.Controllers;

public class GioHangController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<GioHangController> _logger;

    public GioHangController(ApplicationDbContext context, ILogger<GioHangController> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var items = await CurrentCartQuery().AsNoTracking().Include(x => x.SanPham).ToListAsync();
        return View(new GioHangVM
        {
            Items = items.Where(x => x.SanPham != null).Select(x => new GioHangItem
            {
                SanPhamId = x.SanPhamId,
                MaSanPham = x.SanPham!.MaSanPham,
                TenSanPham = x.SanPham.TenSanPham,
                HinhAnh = x.SanPham.HinhAnh,
                DonGia = x.SanPham.GiaKhuyenMai is > 0 ? x.SanPham.GiaKhuyenMai.Value : x.SanPham.GiaBan,
                SoLuong = x.SoLuong,
                SoLuongTon = x.SanPham.SoLuongTon,
                DangBan = x.SanPham.TrangThai
            }).ToList()
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ThemVaoGio(int sanPhamId, int soLuong = 1)
    {
        if (soLuong <= 0)
            return BadRequest(new { success = false, message = "Số lượng phải là số nguyên dương." });

        var product = await _context.SanPhams.AsNoTracking().FirstOrDefaultAsync(x => x.Id == sanPhamId);
        if (product == null)
            return NotFound(new { success = false, message = "Sản phẩm không tồn tại." });
        if (!product.TrangThai)
            return Conflict(new { success = false, message = "Sản phẩm đã ngừng bán." });
        if (product.SoLuongTon <= 0)
            return Conflict(new { success = false, message = "Sản phẩm đã hết hàng." });

        var cartItem = await CurrentCartQuery().FirstOrDefaultAsync(x => x.SanPhamId == sanPhamId);
        var newQuantity = (cartItem?.SoLuong ?? 0) + soLuong;
        if (newQuantity > product.SoLuongTon)
            return Conflict(new { success = false, message = $"Sản phẩm chỉ còn {product.SoLuongTon}; giỏ hiện có {cartItem?.SoLuong ?? 0}." });

        if (cartItem == null)
        {
            var userId = GetUserId();
            cartItem = new GioHang
            {
                SanPhamId = sanPhamId,
                SoLuong = soLuong,
                NguoiDungId = userId,
                SessionId = userId.HasValue ? null : GetSessionId()
            };
            _context.GioHangs.Add(cartItem);
        }
        else
        {
            cartItem.SoLuong = newQuantity;
        }

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            _logger.LogWarning(ex, "Unable to add product {ProductId} to cart", sanPhamId);
            return Conflict(new { success = false, message = "Giỏ hàng vừa được cập nhật ở yêu cầu khác. Vui lòng thử lại." });
        }

        var count = await CurrentCartQuery().SumAsync(x => x.SoLuong);
        return Ok(new { success = true, message = "Đã thêm vào giỏ hàng.", count });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> CapNhatSoLuong(int sanPhamId, int soLuong)
    {
        var cartItem = await CurrentCartQuery().Include(x => x.SanPham).FirstOrDefaultAsync(x => x.SanPhamId == sanPhamId);
        if (cartItem == null)
        {
            TempData["Error"] = "Không tìm thấy sản phẩm trong giỏ hàng của bạn.";
            return RedirectToAction(nameof(Index));
        }

        if (soLuong <= 0)
        {
            _context.GioHangs.Remove(cartItem);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Đã xóa sản phẩm khỏi giỏ hàng.";
            return RedirectToAction(nameof(Index));
        }

        if (cartItem.SanPham == null || !cartItem.SanPham.TrangThai)
            TempData["Error"] = "Sản phẩm đã ngừng bán; vui lòng xóa khỏi giỏ.";
        else if (cartItem.SanPham.SoLuongTon <= 0)
            TempData["Error"] = "Sản phẩm đã hết hàng; vui lòng xóa khỏi giỏ.";
        else if (soLuong > cartItem.SanPham.SoLuongTon)
            TempData["Error"] = $"Sản phẩm chỉ còn {cartItem.SanPham.SoLuongTon}. Số lượng cũ được giữ nguyên.";
        else
        {
            cartItem.SoLuong = soLuong;
            await _context.SaveChangesAsync();
            TempData["Success"] = "Đã cập nhật số lượng.";
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> XoaSanPham(int sanPhamId)
    {
        var cartItem = await CurrentCartQuery().FirstOrDefaultAsync(x => x.SanPhamId == sanPhamId);
        if (cartItem == null)
        {
            TempData["Error"] = "Không tìm thấy sản phẩm trong giỏ hàng của bạn.";
            return RedirectToAction(nameof(Index));
        }

        _context.GioHangs.Remove(cartItem);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Đã xóa sản phẩm khỏi giỏ hàng.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> GetCartCount()
    {
        var count = await CurrentCartQuery().Where(x => x.SoLuong > 0).SumAsync(x => x.SoLuong);
        return Ok(new { count });
    }

    private IQueryable<GioHang> CurrentCartQuery()
    {
        var userId = GetUserId();
        var sessionId = GetSessionId();
        return _context.GioHangs.Where(x => userId.HasValue
            ? x.NguoiDungId == userId.Value
            : x.NguoiDungId == null && x.SessionId == sessionId);
    }

    private string GetSessionId() => HttpContext.Session.Id;
    private int? GetUserId() => HttpContext.Session.GetInt32("UserId");
}

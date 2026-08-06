using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VieITStore.Filters;
using VieITStore.Models;
using VieITStore.Models.Entities;

namespace VieITStore.Areas.Admin.Controllers;

[Area("Admin")]
[AuthorizeRole(VaiTro.Admin, VaiTro.NhanVien)]
public class DonHangAdminController : Controller
{
    private const string CounterChannel = "Tại quầy";
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DonHangAdminController> _logger;

    public DonHangAdminController(ApplicationDbContext context, ILogger<DonHangAdminController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? q, TrangThaiDonHang? trangThai)
    {
        var query = _context.DonHangs
            .AsNoTracking()
            .Include(x => x.KhachHang)
            .ThenInclude(x => x!.NguoiDung)
            .Where(x => x.KenhBan == CounterChannel);

        if (!string.IsNullOrWhiteSpace(q))
        {
            var keyword = q.Trim();
            query = query.Where(x =>
                EF.Functions.Like(x.MaDonHang, $"%{keyword}%")
                || EF.Functions.Like(x.HoTenNguoiNhan, $"%{keyword}%")
                || EF.Functions.Like(x.SoDienThoaiNguoiNhan, $"%{keyword}%")
                || (x.KhachHang!.NguoiDung != null && EF.Functions.Like(x.KhachHang.NguoiDung.HoTen, $"%{keyword}%")));
        }

        if (trangThai.HasValue)
            query = query.Where(x => x.TrangThai == trangThai.Value);

        ViewBag.TuKhoa = q;
        ViewBag.TrangThai = trangThai;
        return View(await query.OrderByDescending(x => x.NgayDat).ToListAsync());
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var order = await _context.DonHangs
            .AsNoTracking()
            .Include(x => x.KhachHang).ThenInclude(x => x!.NguoiDung)
            .Include(x => x.NhanVienXuLy)
            .Include(x => x.ChiTietDonHangs).ThenInclude(x => x.SanPham)
            .SingleOrDefaultAsync(x => x.Id == id && x.KenhBan == CounterChannel);
        return order == null ? NotFound() : View(order);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CapNhatTrangThai(int id, TrangThaiDonHang trangThai, string? lyDoHuy)
    {
        if (!Enum.IsDefined(trangThai))
        {
            TempData["Error"] = "Trạng thái yêu cầu không hợp lệ.";
            return RedirectToAction(nameof(Details), new { id });
        }

        try
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);
            var order = await _context.DonHangs
                .Include(x => x.ChiTietDonHangs)
                .ThenInclude(x => x.SanPham)
                .SingleOrDefaultAsync(x => x.Id == id && x.KenhBan == CounterChannel);

            if (order == null) return NotFound();

            if (order.TrangThai == trangThai)
            {
                TempData["Info"] = "Đơn hàng đã ở trạng thái này, không có thay đổi nào được thực hiện.";
                await transaction.RollbackAsync();
                return RedirectToAction(nameof(Details), new { id });
            }

            if (!IsValidTransition(order.TrangThai, trangThai))
            {
                TempData["Error"] = $"Không thể chuyển từ {StatusText(order.TrangThai)} sang {StatusText(trangThai)}.";
                await transaction.RollbackAsync();
                return RedirectToAction(nameof(Details), new { id });
            }

            if (trangThai == TrangThaiDonHang.DaHuy)
            {
                if (string.IsNullOrWhiteSpace(lyDoHuy))
                {
                    TempData["Error"] = "Vui lòng nhập lý do hủy đơn.";
                    await transaction.RollbackAsync();
                    return RedirectToAction(nameof(Details), new { id });
                }

                foreach (var item in order.ChiTietDonHangs)
                {
                    if (item.SanPham == null)
                        throw new InvalidOperationException($"Không tìm thấy sản phẩm #{item.SanPhamId} để hoàn tồn.");

                    item.SanPham.SoLuongTon = checked(item.SanPham.SoLuongTon + item.SoLuong);
                    item.SanPham.SoLuongDaBan = Math.Max(0, item.SanPham.SoLuongDaBan - item.SoLuong);
                    item.SanPham.NgayCapNhat = DateTime.Now;
                }

                order.LyDoHuy = lyDoHuy.Trim();
            }

            if (trangThai == TrangThaiDonHang.HoanThanh)
            {
                order.DaThanhToan = true;
                order.NgayThanhToan ??= DateTime.Now;
                order.NgayGiaoThucTe ??= DateTime.Now;
            }

            order.TrangThai = trangThai;
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            TempData["Success"] = $"Đã chuyển đơn {order.MaDonHang} sang {StatusText(trangThai)}.";
        }
        catch (OverflowException ex)
        {
            _logger.LogWarning(ex, "Stock overflow while cancelling counter order {OrderId}", id);
            TempData["Error"] = "Không thể hoàn tồn do số lượng vượt giới hạn.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unable to update counter order {OrderId}", id);
            TempData["Error"] = "Không thể cập nhật đơn hàng. Không có thay đổi nào được lưu.";
        }

        return RedirectToAction(nameof(Details), new { id });
    }

    private static bool IsValidTransition(TrangThaiDonHang current, TrangThaiDonHang requested) =>
        (current, requested) switch
        {
            (TrangThaiDonHang.ChoXacNhan, TrangThaiDonHang.DaXacNhan) => true,
            (TrangThaiDonHang.ChoXacNhan, TrangThaiDonHang.DaHuy) => true,
            (TrangThaiDonHang.DaXacNhan, TrangThaiDonHang.HoanThanh) => true,
            (TrangThaiDonHang.DaXacNhan, TrangThaiDonHang.DaHuy) => true,
            (TrangThaiDonHang.DangGiao, TrangThaiDonHang.DaGiao) => true,
            (TrangThaiDonHang.DangGiao, TrangThaiDonHang.DaHuy) => true,
            (TrangThaiDonHang.DaGiao, TrangThaiDonHang.HoanThanh) => true,
            _ => false
        };

    private static string StatusText(TrangThaiDonHang status) => status switch
    {
        TrangThaiDonHang.ChoXacNhan => "Chờ xác nhận",
        TrangThaiDonHang.DaXacNhan => "Đã xác nhận",
        TrangThaiDonHang.DangGiao => "Đang giao",
        TrangThaiDonHang.DaGiao => "Đã giao",
        TrangThaiDonHang.HoanThanh => "Hoàn thành",
        TrangThaiDonHang.DaHuy => "Đã hủy",
        TrangThaiDonHang.YeuCauTraHang => "Yêu cầu trả hàng",
        _ => status.ToString()
    };
}

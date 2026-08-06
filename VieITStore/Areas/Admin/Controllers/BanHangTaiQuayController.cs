using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VieITStore.Filters;
using VieITStore.Models;
using VieITStore.Models.Entities;
using VieITStore.Models.ViewModels;

namespace VieITStore.Areas.Admin.Controllers;

[Area("Admin")]
[AuthorizeRole(VaiTro.Admin, VaiTro.NhanVien)]
public class BanHangTaiQuayController : Controller
{
    private const string RequestTokenSessionKey = "PosOrderRequestToken";
    private readonly ApplicationDbContext _context;
    private readonly ILogger<BanHangTaiQuayController> _logger;

    public BanHangTaiQuayController(ApplicationDbContext context, ILogger<BanHangTaiQuayController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var model = new BanHangTaiQuayVM
        {
            RequestToken = IssueRequestToken(),
            ChiTiets = [new BanHangTaiQuayItemVM { SoLuong = 1 }]
        };
        await LoadFormData();
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BanHangTaiQuayVM model)
    {
        model.ChiTiets ??= [];
        var employeeId = HttpContext.Session.GetInt32("UserId");
        if (!employeeId.HasValue)
            return RedirectToAction("DangNhap", "TaiKhoan", new { area = "" });

        var expectedToken = HttpContext.Session.GetString(RequestTokenSessionKey);
        if (string.IsNullOrEmpty(expectedToken) || !string.Equals(expectedToken, model.RequestToken, StringComparison.Ordinal))
        {
            TempData["Error"] = "Yêu cầu đã được xử lý hoặc hết hiệu lực. Vui lòng tạo lại đơn.";
            return RedirectToAction(nameof(Index));
        }

        if (!Enum.IsDefined(model.PhuongThucThanhToan))
            ModelState.AddModelError(nameof(model.PhuongThucThanhToan), "Phương thức thanh toán không hợp lệ.");

        if (model.ChiTiets.Count != model.ChiTiets.Select(x => x.SanPhamId).Distinct().Count())
            ModelState.AddModelError(nameof(model.ChiTiets), "Không được chọn trùng sản phẩm trong cùng đơn.");

        var customer = await _context.KhachHangs
            .AsNoTracking()
            .Include(x => x.NguoiDung)
            .SingleOrDefaultAsync(x => x.Id == model.KhachHangId && x.NguoiDung!.TrangThai);
        if (customer == null)
            ModelState.AddModelError(nameof(model.KhachHangId), "Khách hàng không tồn tại hoặc đã ngừng hoạt động.");

        if (!ModelState.IsValid)
            return await ReturnInvalidModel(model);

        HttpContext.Session.Remove(RequestTokenSessionKey);

        try
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);
            var productIds = model.ChiTiets.Select(x => x.SanPhamId).ToList();
            var products = await _context.SanPhams
                .Where(x => productIds.Contains(x.Id))
                .ToDictionaryAsync(x => x.Id);

            if (products.Count != productIds.Count)
            {
                ModelState.AddModelError(nameof(model.ChiTiets), "Có sản phẩm không tồn tại.");
                await transaction.RollbackAsync();
                return await ReturnInvalidModel(model);
            }

            var now = DateTime.Now;
            var order = new DonHang
            {
                MaDonHang = $"POS-{now:yyyyMMddHHmmss}-{Guid.NewGuid().ToString("N")[..8]}",
                KenhBan = "Tại quầy",
                KhachHangId = customer!.Id,
                NgayDat = now,
                TrangThai = model.DaThanhToan ? TrangThaiDonHang.HoanThanh : TrangThaiDonHang.DaXacNhan,
                PhuongThucThanhToan = model.PhuongThucThanhToan,
                DaThanhToan = model.DaThanhToan,
                NgayThanhToan = model.DaThanhToan ? now : null,
                NgayGiaoThucTe = model.DaThanhToan ? now : null,
                PhiVanChuyen = 0,
                GiamGia = 0,
                HoTenNguoiNhan = customer.NguoiDung!.HoTen,
                SoDienThoaiNguoiNhan = customer.NguoiDung.SoDienThoai ?? "Chưa cung cấp",
                DiaChiGiaoHang = "Nhận tại cửa hàng",
                GhiChu = string.IsNullOrWhiteSpace(model.GhiChu) ? null : model.GhiChu.Trim(),
                NhanVienXuLyId = employeeId.Value
            };

            decimal total = 0;
            foreach (var item in model.ChiTiets)
            {
                var product = products[item.SanPhamId];
                if (!product.TrangThai)
                {
                    ModelState.AddModelError(nameof(model.ChiTiets), $"Sản phẩm {product.MaSanPham} đã ngừng bán.");
                    await transaction.RollbackAsync();
                    return await ReturnInvalidModel(model);
                }

                if (item.SoLuong > product.SoLuongTon)
                {
                    ModelState.AddModelError(
                        nameof(model.ChiTiets),
                        $"Sản phẩm {product.MaSanPham} - {product.TenSanPham} chỉ còn {product.SoLuongTon}.");
                    await transaction.RollbackAsync();
                    return await ReturnInvalidModel(model);
                }

                var unitPrice = product.GiaKhuyenMai is > 0 ? product.GiaKhuyenMai.Value : product.GiaBan;
                total += unitPrice * item.SoLuong;
                order.ChiTietDonHangs.Add(new ChiTietDonHang
                {
                    SanPhamId = product.Id,
                    SoLuong = item.SoLuong,
                    DonGia = unitPrice
                });

                product.SoLuongTon -= item.SoLuong;
                product.SoLuongDaBan = checked(product.SoLuongDaBan + item.SoLuong);
                product.NgayCapNhat = now;
            }

            order.TongTienHang = total;
            order.TongThanhToan = total;
            _context.DonHangs.Add(order);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            TempData["Success"] = $"Đã tạo đơn tại quầy {order.MaDonHang}, tổng thanh toán {order.TongThanhToan:N0} đ.";
            return RedirectToAction(nameof(Index));
        }
        catch (OverflowException ex)
        {
            _logger.LogWarning(ex, "POS order quantity overflow");
            ModelState.AddModelError(nameof(model.ChiTiets), "Số lượng sản phẩm vượt quá giới hạn cho phép.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unable to create point-of-sale order");
            ModelState.AddModelError(string.Empty, "Không thể tạo đơn. Tồn kho chưa được thay đổi.");
        }

        return await ReturnInvalidModel(model);
    }

    [HttpGet]
    public async Task<IActionResult> TraCuuSanPham(string? code)
    {
        code = code?.Trim();
        if (string.IsNullOrEmpty(code))
            return BadRequest(new { message = "Vui lòng nhập SKU hoặc mã vạch." });

        var product = await _context.SanPhams.AsNoTracking()
            .Where(x => x.MaSanPham == code || x.MaVach == code)
            .Select(x => new
            {
                x.Id,
                x.MaSanPham,
                x.MaVach,
                x.TenSanPham,
                x.SoLuongTon,
                x.TrangThai,
                DonGia = x.GiaKhuyenMai.HasValue && x.GiaKhuyenMai.Value > 0 ? x.GiaKhuyenMai.Value : x.GiaBan
            })
            .FirstOrDefaultAsync();

        if (product == null)
            return NotFound(new { message = "Không tìm thấy sản phẩm theo SKU hoặc mã vạch." });
        if (!product.TrangThai)
            return Conflict(new { message = $"Sản phẩm {product.MaSanPham} đã ngừng bán." });
        if (product.SoLuongTon <= 0)
            return Conflict(new { message = $"Sản phẩm {product.MaSanPham} đã hết hàng." });

        return Ok(new TraCuuSanPhamTaiQuayDTO
        {
            Id = product.Id,
            MaSanPham = product.MaSanPham,
            MaVach = product.MaVach,
            TenSanPham = product.TenSanPham,
            SoLuongTon = product.SoLuongTon,
            DonGia = product.DonGia
        });
    }

    private async Task<IActionResult> ReturnInvalidModel(BanHangTaiQuayVM model)
    {
        model.RequestToken = IssueRequestToken();
        await LoadFormData();
        return View("Index", model);
    }

    private string IssueRequestToken()
    {
        var token = Guid.NewGuid().ToString("N");
        HttpContext.Session.SetString(RequestTokenSessionKey, token);
        return token;
    }

    private async Task LoadFormData()
    {
        ViewBag.KhachHangs = await _context.KhachHangs
            .AsNoTracking()
            .Include(x => x.NguoiDung)
            .Where(x => x.NguoiDung!.TrangThai)
            .OrderBy(x => x.NguoiDung!.HoTen)
            .ToListAsync();
        ViewBag.SanPhams = await _context.SanPhams
            .AsNoTracking()
            .Where(x => x.TrangThai && x.SoLuongTon > 0)
            .OrderBy(x => x.TenSanPham)
            .ToListAsync();
    }
}

using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VieITStore.Models;
using VieITStore.Models.Entities;
using VieITStore.Models.ViewModels;

namespace VieITStore.Controllers;

public class ThanhToanController : Controller
{
    private const decimal ShippingFee = 30000;
    private const string StoreAddress = "123 Nguyễn Văn A, Quận 1, TP.HCM";
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ThanhToanController> _logger;

    public ThanhToanController(ApplicationDbContext context, ILogger<ThanhToanController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue)
            return RedirectToAction("DangNhap", "TaiKhoan", new { returnUrl = Url.Action(nameof(Index)) });

        var cartItems = await LoadCart(userId.Value);
        if (cartItems.Count == 0) return RedirectToAction("Index", "GioHang");
        ValidateCart(cartItems);
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Giỏ hàng có sản phẩm không hợp lệ. Vui lòng kiểm tra lại trước khi thanh toán.";
            return RedirectToAction("Index", "GioHang");
        }

        var customer = await _context.KhachHangs
            .AsNoTracking()
            .Include(x => x.NguoiDung)
            .Include(x => x.DiaChiGiaoHangs)
            .SingleOrDefaultAsync(x => x.NguoiDungId == userId.Value);
        if (customer == null) return NotFound();

        var model = BuildCheckoutModel(new ThanhToanVM
        {
            HoTenNguoiNhan = customer.NguoiDung?.HoTen ?? string.Empty,
            SoDienThoai = customer.NguoiDung?.SoDienThoai ?? string.Empty
        }, cartItems, customer.DiaChiGiaoHangs);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DatHang(ThanhToanVM model)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue)
            return RedirectToAction("DangNhap", "TaiKhoan");

        if (!Enum.IsDefined(model.HinhThucNhanHang))
            ModelState.AddModelError(nameof(model.HinhThucNhanHang), "Hình thức nhận hàng không hợp lệ.");
        if (!Enum.IsDefined(model.PhuongThucThanhToan))
            ModelState.AddModelError(nameof(model.PhuongThucThanhToan), "Phương thức thanh toán không hợp lệ.");

        var isDelivery = model.HinhThucNhanHang == HinhThucNhanHang.GiaoTanNoi;
        if (isDelivery)
        {
            RequireAddress(model.TinhThanh, nameof(model.TinhThanh), "Vui lòng chọn Tỉnh/TP.");
            RequireAddress(model.QuanHuyen, nameof(model.QuanHuyen), "Vui lòng nhập Quận/Huyện.");
            RequireAddress(model.PhuongXa, nameof(model.PhuongXa), "Vui lòng nhập Phường/Xã.");
            RequireAddress(model.DiaChiChiTiet, nameof(model.DiaChiChiTiet), "Vui lòng nhập địa chỉ nhận hàng.");
        }
        else
        {
            ModelState.Remove(nameof(model.TinhThanh));
            ModelState.Remove(nameof(model.QuanHuyen));
            ModelState.Remove(nameof(model.PhuongXa));
            ModelState.Remove(nameof(model.DiaChiChiTiet));
        }

        var customer = await _context.KhachHangs
            .Include(x => x.DiaChiGiaoHangs)
            .SingleOrDefaultAsync(x => x.NguoiDungId == userId.Value);
        if (customer == null) return NotFound();

        var cartItems = await LoadCart(userId.Value);
        ValidateCart(cartItems);
        if (!ModelState.IsValid)
            return View("Index", BuildCheckoutModel(model, cartItems, customer.DiaChiGiaoHangs));

        try
        {
            _context.ChangeTracker.Clear();
            await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);
            cartItems = await LoadCart(userId.Value);
            ValidateCart(cartItems);
            if (!ModelState.IsValid)
            {
                await transaction.RollbackAsync();
                return View("Index", BuildCheckoutModel(model, cartItems, customer.DiaChiGiaoHangs));
            }

            var now = DateTime.Now;
            var order = new DonHang
            {
                MaDonHang = $"VIE-{now:yyyyMMddHHmmss}-{Guid.NewGuid().ToString("N")[..8]}",
                KenhBan = "Trực tuyến",
                KhachHangId = customer.Id,
                HinhThucNhanHang = model.HinhThucNhanHang,
                PhuongThucThanhToan = model.PhuongThucThanhToan,
                HoTenNguoiNhan = model.HoTenNguoiNhan.Trim(),
                SoDienThoaiNguoiNhan = model.SoDienThoai.Trim(),
                DiaChiGiaoHang = isDelivery
                    ? $"{model.DiaChiChiTiet.Trim()}, {model.PhuongXa.Trim()}, {model.QuanHuyen.Trim()}, {model.TinhThanh.Trim()}"
                    : $"Khách nhận tại cửa hàng VieITStore - {StoreAddress}",
                GhiChu = string.IsNullOrWhiteSpace(model.GhiChu) ? null : model.GhiChu.Trim(),
                PhiVanChuyen = isDelivery ? ShippingFee : 0,
                NgayGiaoDuKien = isDelivery ? now.AddDays(3) : null,
                NgayDat = now,
                TrangThai = TrangThaiDonHang.ChoXacNhan,
                DaThanhToan = false
            };

            decimal subtotal = 0;
            foreach (var item in cartItems)
            {
                var product = item.SanPham!;
                var unitPrice = product.GiaKhuyenMai is > 0 ? product.GiaKhuyenMai.Value : product.GiaBan;
                subtotal += unitPrice * item.SoLuong;
                order.ChiTietDonHangs.Add(new ChiTietDonHang
                {
                    SanPhamId = item.SanPhamId,
                    DonGia = unitPrice,
                    SoLuong = item.SoLuong
                });
                product.SoLuongTon -= item.SoLuong;
                product.SoLuongDaBan = checked(product.SoLuongDaBan + item.SoLuong);
                product.NgayCapNhat = now;
            }

            order.TongTienHang = subtotal;
            order.TongThanhToan = subtotal + order.PhiVanChuyen;
            _context.DonHangs.Add(order);
            _context.GioHangs.RemoveRange(cartItems);

            if (isDelivery && model.LuuDiaChi)
            {
                _context.DiaChiGiaoHangs.Add(new DiaChiGiaoHang
                {
                    KhachHangId = customer.Id,
                    HoTenNguoiNhan = model.HoTenNguoiNhan.Trim(),
                    SoDienThoai = model.SoDienThoai.Trim(),
                    TinhThanh = model.TinhThanh.Trim(),
                    QuanHuyen = model.QuanHuyen.Trim(),
                    PhuongXa = model.PhuongXa.Trim(),
                    DiaChiChiTiet = model.DiaChiChiTiet.Trim()
                });
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            TempData["Success"] = $"Đặt hàng thành công! Mã đơn: {order.MaDonHang}";
            return RedirectToAction("ChiTiet", "DonHang", new { id = order.Id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unable to place online order");
            _context.ChangeTracker.Clear();
            ModelState.AddModelError(string.Empty, "Không thể đặt hàng. Tồn kho chưa được thay đổi.");
            cartItems = await LoadCart(userId.Value);
            return View("Index", BuildCheckoutModel(model, cartItems, customer.DiaChiGiaoHangs));
        }
    }

    private void RequireAddress(string? value, string key, string message)
    {
        if (string.IsNullOrWhiteSpace(value)) ModelState.AddModelError(key, message);
    }

    private void ValidateCart(List<GioHang> cartItems)
    {
        if (cartItems.Count == 0)
        {
            ModelState.AddModelError(string.Empty, "Giỏ hàng trống.");
            return;
        }

        foreach (var item in cartItems)
        {
            if (item.SanPham == null || !item.SanPham.TrangThai)
                ModelState.AddModelError(string.Empty, "Có sản phẩm không còn được bán.");
            else if (item.SoLuong <= 0)
                ModelState.AddModelError(string.Empty, $"Số lượng của {item.SanPham.TenSanPham} không hợp lệ.");
            else if (item.SoLuong > item.SanPham.SoLuongTon)
                ModelState.AddModelError(string.Empty, $"Sản phẩm {item.SanPham.TenSanPham} chỉ còn {item.SanPham.SoLuongTon}.");
        }
    }

    private Task<List<GioHang>> LoadCart(int userId) => _context.GioHangs
        .Include(x => x.SanPham)
        .Where(x => x.NguoiDungId == userId)
        .ToListAsync();

    private static ThanhToanVM BuildCheckoutModel(
        ThanhToanVM model,
        IEnumerable<GioHang> cartItems,
        IEnumerable<DiaChiGiaoHang> addresses)
    {
        model.GioHang = new GioHangVM
        {
            Items = cartItems.Where(x => x.SanPham != null).Select(x => new GioHangItem
            {
                SanPhamId = x.SanPhamId,
                MaSanPham = x.SanPham!.MaSanPham,
                TenSanPham = x.SanPham!.TenSanPham,
                HinhAnh = x.SanPham.HinhAnh,
                DonGia = x.SanPham.GiaKhuyenMai is > 0 ? x.SanPham.GiaKhuyenMai.Value : x.SanPham.GiaBan,
                SoLuong = x.SoLuong,
                SoLuongTon = x.SanPham.SoLuongTon,
                DangBan = x.SanPham.TrangThai
            }).ToList()
        };
        model.DiaChiGiaoHangs = addresses.ToList();
        return model;
    }
}

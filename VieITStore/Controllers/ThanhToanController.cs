using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VieITStore.Models;
using VieITStore.Models.Entities;
using VieITStore.Models.ViewModels;

namespace VieITStore.Controllers
{
    public class ThanhToanController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ThanhToanController> _logger;

        public ThanhToanController(ApplicationDbContext context, ILogger<ThanhToanController> logger)
        {
            _context = context;
            _logger = logger;
        }

        private int? GetUserId() => HttpContext.Session.GetInt32("UserId");

        public async Task<IActionResult> Index()
        {
            try
            {
                var userId = GetUserId();
                if (!userId.HasValue) return RedirectToAction("DangNhap", "TaiKhoan");

                var gioHangItems = await _context.GioHangs
                    .Include(g => g.SanPham)
                    .Where(g => g.NguoiDungId == userId)
                    .ToListAsync();

                if (!gioHangItems.Any()) return RedirectToAction("Index", "GioHang");

                var khachHang = await _context.KhachHangs
                    .Include(k => k.DiaChiGiaoHangs)
                    .FirstOrDefaultAsync(k => k.NguoiDungId == userId);

                var vm = new ThanhToanVM
                {
                    GioHang = new GioHangVM
                    {
                        Items = gioHangItems.Select(g => new GioHangItem
                        {
                            SanPhamId = g.SanPhamId,
                            TenSanPham = g.SanPham!.TenSanPham,
                            HinhAnh = g.SanPham.HinhAnh,
                            DonGia = g.SanPham.GiaKhuyenMai ?? g.SanPham.GiaBan,
                            SoLuong = g.SoLuong
                        }).ToList()
                    },
                    DiaChiGiaoHangs = khachHang?.DiaChiGiaoHangs.ToList() ?? new List<DiaChiGiaoHang>()
                };

                return View(vm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ThanhToan Index");
                return StatusCode(500, "Có lỗi xảy ra");
            }
        }

        [HttpPost]
        public async Task<IActionResult> DatHang(ThanhToanVM model)
        {
            try
            {
                var userId = GetUserId();
                if (!userId.HasValue) return RedirectToAction("DangNhap", "TaiKhoan");

                var khachHang = await _context.KhachHangs
                    .FirstOrDefaultAsync(k => k.NguoiDungId == userId);

                if (khachHang == null) return NotFound();

                var gioHangItems = await _context.GioHangs
                    .Include(g => g.SanPham)
                    .Where(g => g.NguoiDungId == userId)
                    .ToListAsync();

                if (!gioHangItems.Any())
                {
                    ModelState.AddModelError("", "Giỏ hàng trống!");
                    return View("Index", model);
                }

                // Kiểm tra tồn kho
                foreach (var item in gioHangItems)
                {
                    if (item.SoLuong > item.SanPham!.SoLuongTon)
                    {
                        ModelState.AddModelError("", $"Sản phẩm {item.SanPham.TenSanPham} chỉ còn {item.SanPham.SoLuongTon}!");
                        return View("Index", model);
                    }
                }

                var donHang = new DonHang
                {
                    MaDonHang = $"VIE{DateTime.Now:yyyyMMdd}{new Random().Next(1000, 9999)}",
                    KhachHangId = khachHang.Id,
                    PhuongThucThanhToan = model.PhuongThucThanhToan,
                    HoTenNguoiNhan = model.HoTenNguoiNhan,
                    SoDienThoaiNguoiNhan = model.SoDienThoai,
                    DiaChiGiaoHang = $"{model.DiaChiChiTiet}, {model.PhuongXa}, {model.QuanHuyen}, {model.TinhThanh}",
                    GhiChu = model.GhiChu,
                    PhiVanChuyen = 30000,
                    NgayGiaoDuKien = DateTime.Now.AddDays(3),
                    NgayDat = DateTime.Now,
                    TrangThai = TrangThaiDonHang.ChoXacNhan
                };

                decimal tongTien = 0;
                foreach (var item in gioHangItems)
                {
                    var donGia = item.SanPham!.GiaKhuyenMai ?? item.SanPham.GiaBan;
                    tongTien += donGia * item.SoLuong;

                    donHang.ChiTietDonHangs.Add(new ChiTietDonHang
                    {
                        SanPhamId = item.SanPhamId,
                        DonGia = donGia,
                        SoLuong = item.SoLuong
                    });

                    item.SanPham.SoLuongTon -= item.SoLuong;
                    item.SanPham.SoLuongDaBan += item.SoLuong;
                    item.SanPham.NgayCapNhat = DateTime.Now;
                }

                donHang.TongTienHang = tongTien;
                donHang.TongThanhToan = tongTien + donHang.PhiVanChuyen - donHang.GiamGia;

                if (model.PhuongThucThanhToan == PhuongThucThanhToan.COD)
                    donHang.DaThanhToan = false;

                _context.DonHangs.Add(donHang);
                _context.GioHangs.RemoveRange(gioHangItems);

                if (model.LuuDiaChi)
                {
                    _context.DiaChiGiaoHangs.Add(new DiaChiGiaoHang
                    {
                        KhachHangId = khachHang.Id,
                        HoTenNguoiNhan = model.HoTenNguoiNhan,
                        SoDienThoai = model.SoDienThoai,
                        TinhThanh = model.TinhThanh,
                        QuanHuyen = model.QuanHuyen,
                        PhuongXa = model.PhuongXa,
                        DiaChiChiTiet = model.DiaChiChiTiet
                    });
                }

                await _context.SaveChangesAsync();

                TempData["Success"] = $"Đặt hàng thành công! Mã đơn: {donHang.MaDonHang}";
                return RedirectToAction("ChiTiet", "DonHang", new { id = donHang.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DatHang");
                ModelState.AddModelError("", $"Lỗi: {ex.Message}");
                return View("Index", model);
            }
        }
    }
}
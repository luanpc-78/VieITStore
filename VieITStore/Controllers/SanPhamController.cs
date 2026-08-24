using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VieITStore.Models;
using VieITStore.Models.Entities;
using VieITStore.Models.ViewModels;

namespace VieITStore.Controllers
{
    public class SanPhamController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<SanPhamController> _logger;

        public SanPhamController(ApplicationDbContext context, ILogger<SanPhamController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index(int? danhMucId, string? search, string? sort)
        {
            try
            {
                var query = _context.SanPhams
                    .Include(s => s.DanhMuc)
                    .Where(s => s.TrangThai)
                    .AsQueryable();

                if (danhMucId.HasValue)
                    query = query.Where(s => s.DanhMucId == danhMucId);

                if (!string.IsNullOrEmpty(search))
                    query = query.Where(s => s.TenSanPham.Contains(search) || s.MaSanPham.Contains(search));

                query = sort switch
                {
                    "gia-thap" => query.OrderBy(s => s.GiaBan),
                    "gia-cao" => query.OrderByDescending(s => s.GiaBan),
                    "ban-chay" => query.OrderByDescending(s => s.SoLuongDaBan),
                    _ => query.OrderByDescending(s => s.NgayTao)
                };

                ViewBag.DanhMucs = await _context.DanhMucs.Where(d => d.HienThi).ToListAsync();
                ViewBag.DanhMucId = danhMucId;
                ViewBag.CurrentSort = sort;
                ViewBag.Search = search;

                return View(await query.ToListAsync());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SanPham Index");
                return StatusCode(500, "Có lỗi xảy ra");
            }
        }

        public async Task<IActionResult> ChiTiet(int? id)
        {
            try
            {
                if (id == null) return NotFound();

                var sanPham = await _context.SanPhams
                    .Include(s => s.DanhMuc)
                    .FirstOrDefaultAsync(s => s.Id == id && s.TrangThai);

                if (sanPham == null) return NotFound();

                ViewBag.SanPhamLienQuan = await _context.SanPhams
                    .Where(s => s.DanhMucId == sanPham.DanhMucId && s.Id != id && s.TrangThai)
                    .Take(4)
                    .ToListAsync();

                var visibleReviews = await _context.DanhGias.AsNoTracking()
                    .Include(x => x.KhachHang).ThenInclude(x => x!.NguoiDung)
                    .Where(x => x.SanPhamId == id && x.HienThi)
                    .OrderByDescending(x => x.NgayDanhGia).ToListAsync();
                var reviewVm = new DanhGiaSanPhamVM
                {
                    DanhGias = visibleReviews,
                    TongDanhGia = visibleReviews.Count,
                    DiemTrungBinh = visibleReviews.Count == 0 ? 0 : visibleReviews.Average(x => x.SoSao)
                };
                var userId = HttpContext.Session.GetInt32("UserId");
                if (userId.HasValue)
                {
                    var customer = await _context.KhachHangs.SingleOrDefaultAsync(x => x.NguoiDungId == userId);
                    if (customer != null)
                    {
                        reviewVm.DanhGiaCuaToi = await _context.DanhGias.AsNoTracking().SingleOrDefaultAsync(x => x.KhachHangId == customer.Id && x.SanPhamId == id);
                        reviewVm.DuocDanhGia = await _context.ChiTietDonHangs.AnyAsync(x => x.SanPhamId == id && x.DonHang!.KhachHangId == customer.Id && x.DonHang.TrangThai == TrangThaiDonHang.HoanThanh);
                    }
                }
                ViewBag.DanhGia = reviewVm;

                return View(sanPham);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SanPham ChiTiet");
                return StatusCode(500, "Có lỗi xảy ra");
            }
        }
    }
}

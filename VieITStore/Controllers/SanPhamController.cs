using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VieITStore.Models;

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
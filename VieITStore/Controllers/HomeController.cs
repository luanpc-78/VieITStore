using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VieITStore.Models;

namespace VieITStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var sanPhamNoiBat = await _context.SanPhams
                .Include(s => s.DanhMuc)
                .Where(s => s.NoiBat && s.TrangThai)
                .Take(8)
                .ToListAsync();

            var sanPhamMoi = await _context.SanPhams
                .Include(s => s.DanhMuc)
                .Where(s => s.TrangThai)
                .OrderByDescending(s => s.NgayTao)
                .Take(8)
                .ToListAsync();

            ViewBag.DanhMucs = await _context.DanhMucs
                .Where(d => d.HienThi)
                .ToListAsync();

            ViewBag.SanPhamMoi = sanPhamMoi;
            return View(sanPhamNoiBat);
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            Response.StatusCode = StatusCodes.Status500InternalServerError;
            return View();
        }
    }
}

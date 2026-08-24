using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VieITStore.Models;
using VieITStore.Models.ViewModels;

namespace VieITStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public HomeController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
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

            var danhMucs = await _context.DanhMucs
                .Where(d => d.HienThi)
                .ToListAsync();

            var ungVienDeXuat = await _context.SanPhams
                .AsNoTracking()
                .Include(s => s.DanhMuc)
                .Where(s => s.TrangThai && s.SoLuongTon > 0 && s.HinhAnh != null && s.HinhAnh != "")
                .OrderByDescending(s => s.NoiBat)
                .ThenByDescending(s => s.NgayTao)
                .ToListAsync();
            var sanPhamDeXuat = ungVienDeXuat
                .Where(s => IsValidProductImage(s.HinhAnh))
                .Take(8)
                .ToList();

            return View(new HomeVM
            {
                SanPhamNoiBat = sanPhamNoiBat,
                SanPhamMoi = sanPhamMoi,
                SanPhamDeXuat = sanPhamDeXuat,
                DanhMucs = danhMucs
            });
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        private bool IsValidProductImage(string? imagePath)
        {
            if (string.IsNullOrWhiteSpace(imagePath)) return false;
            if (Uri.TryCreate(imagePath, UriKind.Absolute, out var uri))
                return uri.Scheme is "http" or "https";

            var relativePath = imagePath.Trim().TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
            var fullPath = Path.GetFullPath(Path.Combine(_environment.WebRootPath, relativePath));
            var webRoot = Path.GetFullPath(_environment.WebRootPath) + Path.DirectorySeparatorChar;
            return fullPath.StartsWith(webRoot, StringComparison.OrdinalIgnoreCase) && System.IO.File.Exists(fullPath);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            Response.StatusCode = StatusCodes.Status500InternalServerError;
            return View();
        }
    }
}

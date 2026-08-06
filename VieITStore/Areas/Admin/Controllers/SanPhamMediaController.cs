using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VieITStore.Filters;
using VieITStore.Models;
using VieITStore.Models.Entities;

namespace VieITStore.Areas.Admin.Controllers;

[Area("Admin")]
[AuthorizeRole(VaiTro.Admin, VaiTro.NhanVien)]
public class SanPhamMediaController : Controller
{
    private static readonly HashSet<string> AllowedExtensions =
        new(StringComparer.OrdinalIgnoreCase) { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<SanPhamMediaController> _logger;

    public SanPhamMediaController(
        ApplicationDbContext context,
        IWebHostEnvironment environment,
        ILogger<SanPhamMediaController> logger)
    {
        _context = context;
        _environment = environment;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index(int id)
    {
        var product = await _context.SanPhams
            .AsNoTracking()
            .Include(x => x.DanhMuc)
            .SingleOrDefaultAsync(x => x.Id == id);
        return product == null ? NotFound() : View(product);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CapNhatThongSo(int id, string? thongSoKyThuat)
    {
        var product = await _context.SanPhams.FindAsync(id);
        if (product == null) return NotFound();

        product.ThongSoKyThuat = string.IsNullOrWhiteSpace(thongSoKyThuat)
            ? null
            : thongSoKyThuat.Trim();
        product.NgayCapNhat = DateTime.Now;
        await _context.SaveChangesAsync();

        TempData["Success"] = "Đã cập nhật thông số kỹ thuật.";
        return RedirectToAction(nameof(Index), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ThemAnh(int id, List<IFormFile>? hinhAnhBoSung)
    {
        var product = await _context.SanPhams.FindAsync(id);
        if (product == null) return NotFound();

        var images = ParseImages(product.HinhAnhBoSung);
        var initialImageCount = images.Count;
        var availableSlots = 5 - images.Count;
        if (availableSlots <= 0)
        {
            TempData["Error"] = "Sản phẩm đã có tối đa 5 ảnh bổ sung.";
            return RedirectToAction(nameof(Index), new { id });
        }

        foreach (var file in (hinhAnhBoSung ?? []).Take(availableSlots))
        {
            var path = await SaveUploadedFile(file);
            if (path != null) images.Add(path);
        }

        product.HinhAnhBoSung = images.Count == 0 ? null : string.Join('|', images);
        product.NgayCapNhat = DateTime.Now;
        await _context.SaveChangesAsync();

        var addedImage = images.Count > initialImageCount;
        TempData[addedImage ? "Success" : "Error"] = addedImage
            ? "Đã cập nhật thư viện ảnh sản phẩm."
            : "Không có ảnh hợp lệ để tải lên.";
        return RedirectToAction(nameof(Index), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> XoaAnh(int id, string imagePath)
    {
        var product = await _context.SanPhams.FindAsync(id);
        if (product == null) return NotFound();

        var images = ParseImages(product.HinhAnhBoSung);
        if (!images.Remove(imagePath)) return NotFound();

        product.HinhAnhBoSung = images.Count == 0 ? null : string.Join('|', images);
        product.NgayCapNhat = DateTime.Now;
        await _context.SaveChangesAsync();
        DeleteUploadedFile(imagePath);

        TempData["Success"] = "Đã xóa ảnh khỏi thư viện sản phẩm.";
        return RedirectToAction(nameof(Index), new { id });
    }

    private async Task<string?> SaveUploadedFile(IFormFile file)
    {
        if (file.Length == 0 || file.Length > 5 * 1024 * 1024) return null;
        var extension = Path.GetExtension(file.FileName);
        if (!AllowedExtensions.Contains(extension)) return null;

        var uploadDirectory = Path.Combine(_environment.WebRootPath, "uploads", "products");
        Directory.CreateDirectory(uploadDirectory);
        var fileName = $"prod_gallery_{Guid.NewGuid():N}{extension.ToLowerInvariant()}";
        var filePath = Path.Combine(uploadDirectory, fileName);

        try
        {
            await using var stream = new FileStream(filePath, FileMode.CreateNew);
            await file.CopyToAsync(stream);
            return $"/uploads/products/{fileName}";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unable to save gallery image {FileName}", fileName);
            return null;
        }
    }

    private void DeleteUploadedFile(string imagePath)
    {
        try
        {
            var uploadRoot = Path.GetFullPath(Path.Combine(_environment.WebRootPath, "uploads", "products"));
            var candidate = Path.GetFullPath(Path.Combine(_environment.WebRootPath, imagePath.TrimStart('/')));
            if (candidate.StartsWith(uploadRoot + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase)
                && System.IO.File.Exists(candidate))
                System.IO.File.Delete(candidate);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Unable to delete gallery image {ImagePath}", imagePath);
        }
    }

    private static List<string> ParseImages(string? value) => string.IsNullOrWhiteSpace(value)
        ? []
        : value.Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Take(5)
            .ToList();
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VieITStore.Models;
using VieITStore.Models.Entities;
using VieITStore.Filters;

namespace VieITStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AuthorizeRole(VaiTro.Admin, VaiTro.NhanVien)]
    public class SanPhamAdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<SanPhamAdminController> _logger;
        private readonly IWebHostEnvironment _environment;

        public SanPhamAdminController(ApplicationDbContext context, ILogger<SanPhamAdminController> logger, IWebHostEnvironment environment)
        {
            _context = context;
            _logger = logger;
            _environment = environment;
        }

        private async Task<string?> SaveUploadedFile(IFormFile? file)
        {
            if (file == null || file.Length == 0)
                return null;

            try
            {
                // Kiểm tra loại file
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                var fileExtension = Path.GetExtension(file.FileName).ToLower();

                if (!allowedExtensions.Contains(fileExtension))
                {
                    _logger.LogWarning($"Invalid file extension: {fileExtension}");
                    return null;
                }

                // Kiểm tra kích thước (max 5MB)
                if (file.Length > 5 * 1024 * 1024)
                {
                    _logger.LogWarning("File size exceeds 5MB limit");
                    return null;
                }

                // Tạo thư mục nếu chưa tồn tại
                var uploadDir = Path.Combine(_environment.WebRootPath, "uploads", "products");
                if (!Directory.Exists(uploadDir))
                    Directory.CreateDirectory(uploadDir);

                // Tạo tên file duy nhất
                var fileName = $"prod_{DateTime.Now:yyyyMMddHHmmss}_{Guid.NewGuid().ToString().Substring(0, 8)}{fileExtension}";
                var filePath = Path.Combine(uploadDir, fileName);

                // Lưu file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                return $"/uploads/products/{fileName}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving uploaded file");
                return null;
            }
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var sanPhams = await _context.SanPhams
                    .Include(s => s.DanhMuc)
                    .OrderByDescending(s => s.NgayTao)
                    .ToListAsync();

                return View(sanPhams);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Index");
                return StatusCode(500, "Có lỗi xảy ra");
            }
        }

        public async Task<IActionResult> Create()
        {
            try
            {
                ViewBag.DanhMucs = await _context.DanhMucs.ToListAsync();
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Create GET");
                return StatusCode(500, "Có lỗi xảy ra");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("MaSanPham,MaVach,TenSanPham,DanhMucId,ThuongHieu,GiaNhap,GiaBan,GiaKhuyenMai,SoLuongTon,BaoHanh,XuatXu,MoTaNgan,MoTaChiTiet,NoiBat,TrangThai")]
            SanPham sanPham,
            IFormFile? hinhAnh)
        {
            try
            {
                sanPham.MaSanPham = sanPham.MaSanPham.Trim();
                sanPham.MaVach = string.IsNullOrWhiteSpace(sanPham.MaVach) ? null : sanPham.MaVach.Trim();
                if (await _context.SanPhams.AnyAsync(x => x.MaSanPham == sanPham.MaSanPham))
                    ModelState.AddModelError(nameof(sanPham.MaSanPham), "Mã sản phẩm đã tồn tại.");
                if (sanPham.MaVach != null && await _context.SanPhams.AnyAsync(x => x.MaVach == sanPham.MaVach))
                    ModelState.AddModelError(nameof(sanPham.MaVach), "Mã vạch đã tồn tại.");

                // Log ModelState errors
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors);
                    foreach (var error in errors)
                    {
                        _logger.LogWarning($"Validation Error: {error.ErrorMessage}");
                    }

                }

                if (ModelState.IsValid)
                {
                    // Xử lý upload ảnh
                    if (hinhAnh != null && hinhAnh.Length > 0)
                    {
                        var imagePath = await SaveUploadedFile(hinhAnh);
                        if (imagePath != null)
                            sanPham.HinhAnh = imagePath;
                    }

                    // Tạo slug từ tên sản phẩm
                    if (string.IsNullOrEmpty(sanPham.Slug))
                    {
                        sanPham.Slug = sanPham.TenSanPham?.ToLower().Replace(" ", "-") ?? "";
                    }

                    // Gán giá trị mặc định nếu chưa có
                    if (sanPham.GiaNhap == 0) sanPham.GiaNhap = sanPham.GiaBan;
                    if (sanPham.NgayTao == default) sanPham.NgayTao = DateTime.Now;
                    if (sanPham.NgayCapNhat == default) sanPham.NgayCapNhat = DateTime.Now;

                    _context.SanPhams.Add(sanPham);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Thêm sản phẩm thành công!";
                    return RedirectToAction("Index");
                }

                // Nếu validation fail, trả về form với lỗi
                ViewBag.DanhMucs = await _context.DanhMucs.ToListAsync();
                return View(sanPham);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Create POST");
                ModelState.AddModelError("", $"Lỗi: {ex.Message}");
                ViewBag.DanhMucs = await _context.DanhMucs.ToListAsync();
                return View(sanPham);
            }
        }

        public async Task<IActionResult> Edit(int? id)
        {
            try
            {
                if (id == null) return NotFound();
                var sanPham = await _context.SanPhams.FindAsync(id);
                if (sanPham == null) return NotFound();
                ViewBag.DanhMucs = await _context.DanhMucs.ToListAsync();
                return View(sanPham);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Edit GET");
                return StatusCode(500, "Có lỗi xảy ra");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("Id,MaSanPham,MaVach,TenSanPham,DanhMucId,ThuongHieu,GiaNhap,GiaBan,GiaKhuyenMai,SoLuongTon,BaoHanh,XuatXu,MoTaNgan,MoTaChiTiet,NoiBat,TrangThai")]
            SanPham sanPham,
            IFormFile? hinhAnh)
        {
            try
            {
                if (id != sanPham.Id) return NotFound();
                sanPham.MaSanPham = sanPham.MaSanPham.Trim();
                sanPham.MaVach = string.IsNullOrWhiteSpace(sanPham.MaVach) ? null : sanPham.MaVach.Trim();
                if (await _context.SanPhams.AnyAsync(x => x.Id != id && x.MaSanPham == sanPham.MaSanPham))
                    ModelState.AddModelError(nameof(sanPham.MaSanPham), "Mã sản phẩm đã tồn tại.");
                if (sanPham.MaVach != null && await _context.SanPhams.AnyAsync(x => x.Id != id && x.MaVach == sanPham.MaVach))
                    ModelState.AddModelError(nameof(sanPham.MaVach), "Mã vạch đã tồn tại.");

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors);
                    foreach (var error in errors)
                    {
                        _logger.LogWarning($"Validation Error: {error.ErrorMessage}");
                    }

                }

                if (ModelState.IsValid)
                {
                    var existing = await _context.SanPhams.FindAsync(id);
                    if (existing == null) return NotFound();

                    // Xử lý upload ảnh mới
                    if (hinhAnh != null && hinhAnh.Length > 0)
                    {
                        var imagePath = await SaveUploadedFile(hinhAnh);
                        if (imagePath != null)
                        {
                            // Xóa ảnh cũ nếu tồn tại
                            if (!string.IsNullOrEmpty(existing.HinhAnh))
                            {
                                try
                                {
                                    var oldFilePath = Path.Combine(_environment.WebRootPath, existing.HinhAnh.TrimStart('/'));
                                    if (System.IO.File.Exists(oldFilePath))
                                        System.IO.File.Delete(oldFilePath);
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogWarning(ex, "Error deleting old image");
                                }
                            }

                            existing.HinhAnh = imagePath;
                        }
                    }

                    // Cập nhật các thuộc tính
                    existing.MaSanPham = sanPham.MaSanPham;
                    existing.MaVach = sanPham.MaVach;
                    existing.TenSanPham = sanPham.TenSanPham;
                    existing.DanhMucId = sanPham.DanhMucId;
                    existing.GiaNhap = sanPham.GiaNhap;
                    existing.GiaBan = sanPham.GiaBan;
                    existing.GiaKhuyenMai = sanPham.GiaKhuyenMai;
                    existing.SoLuongTon = sanPham.SoLuongTon;
                    existing.BaoHanh = sanPham.BaoHanh;
                    existing.XuatXu = sanPham.XuatXu;
                    existing.ThuongHieu = sanPham.ThuongHieu;
                    existing.MoTaNgan = sanPham.MoTaNgan;
                    existing.MoTaChiTiet = sanPham.MoTaChiTiet;
                    existing.NoiBat = sanPham.NoiBat;
                    existing.TrangThai = sanPham.TrangThai;
                    existing.Slug = sanPham.TenSanPham.ToLower().Replace(" ", "-");
                    existing.NgayCapNhat = DateTime.Now;

                    _context.Update(existing);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Cập nhật thành công!";
                    return RedirectToAction("Index");
                }

                ViewBag.DanhMucs = await _context.DanhMucs.ToListAsync();
                return View(sanPham);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Edit POST");
                ModelState.AddModelError("", $"Lỗi: {ex.Message}");
                ViewBag.DanhMucs = await _context.DanhMucs.ToListAsync();
                return View(sanPham);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NgungBan(int id)
        {
            try
            {
                var sanPham = await _context.SanPhams.FindAsync(id);
                if (sanPham == null) return NotFound();

                sanPham.TrangThai = false;
                sanPham.NgayCapNhat = DateTime.Now;
                await _context.SaveChangesAsync();
                TempData["Success"] = "Sản phẩm đã được chuyển sang trạng thái ngừng bán.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in NgungBan");
                TempData["Error"] = $"Không thể ngừng bán sản phẩm: {ex.Message}";
                return RedirectToAction("Index");
            }
        }
    }
}

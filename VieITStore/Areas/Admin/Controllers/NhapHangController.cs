using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using VieITStore.Filters;
using VieITStore.Models;
using VieITStore.Models.Entities;

namespace VieITStore.Areas.Admin.Controllers;

[Area("Admin")]
[AuthorizeRole(VaiTro.Admin, VaiTro.NhanVien)]
public class NhapHangController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<NhapHangController> _logger;

    public NhapHangController(ApplicationDbContext context, ILogger<NhapHangController> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var phieuNhaps = await _context.PhieuNhaps
                .Include(p => p.NhaCungCap)
                .Include(p => p.NguoiNhap)
                .OrderByDescending(p => p.NgayNhap)
                .ToListAsync();

            return View(phieuNhaps);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in NhapHang Index");
            return StatusCode(500, "Có lỗi xảy ra");
        }
    }

    public async Task<IActionResult> Create()
    {
        try
        {
            await LoadSelections();
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
    public async Task<IActionResult> Create(PhieuNhap phieuNhap, List<ChiTietPhieuNhap>? chiTiets, List<string?>? serialInputs)
    {
        try
        {
            var nguoiNhapId = HttpContext.Session.GetInt32("UserId");
            if (nguoiNhapId == null)
                return RedirectToAction("DangNhap", "TaiKhoan", new { area = "" });

            phieuNhap.MaPhieuNhap = $"PN{DateTime.Now:yyyyMMddHHmmssfff}";
            phieuNhap.NguoiNhapId = nguoiNhapId.Value;
            phieuNhap.NgayNhap = DateTime.Now;
            ModelState.Remove(nameof(phieuNhap.MaPhieuNhap));

            if (!await _context.NhaCungCaps.AnyAsync(x => x.Id == phieuNhap.NhaCungCapId && x.TrangThai))
                ModelState.AddModelError(nameof(phieuNhap.NhaCungCapId), "Nhà cung cấp không tồn tại hoặc đã ngừng hợp tác.");

            var details = chiTiets ?? [];
            if (details.Count == 0)
                ModelState.AddModelError(string.Empty, "Vui lòng thêm ít nhất một sản phẩm.");

            var productIds = details.Select(x => x.SanPhamId).Distinct().ToList();
            var products = await _context.SanPhams
                .Where(x => productIds.Contains(x.Id) && x.TrangThai)
                .ToDictionaryAsync(x => x.Id);
            var serialsByRow = new List<List<string>>();
            var allSerials = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            for (var index = 0; index < details.Count; index++)
            {
                var detail = details[index];
                if (!products.ContainsKey(detail.SanPhamId))
                    ModelState.AddModelError(string.Empty, $"Sản phẩm ở dòng {index + 1} không tồn tại hoặc đã ngừng bán.");
                if (detail.SoLuong <= 0)
                    ModelState.AddModelError(string.Empty, $"Số lượng ở dòng {index + 1} phải lớn hơn 0.");
                if (detail.DonGiaNhap <= 0)
                    ModelState.AddModelError(string.Empty, $"Đơn giá nhập ở dòng {index + 1} phải lớn hơn 0.");

                var rowSerials = ParseSerials(serialInputs?.ElementAtOrDefault(index));
                if (rowSerials.Count > detail.SoLuong)
                    ModelState.AddModelError(string.Empty, $"Dòng {index + 1} có nhiều serial/IMEI hơn số lượng nhập.");
                foreach (var serial in rowSerials)
                {
                    if (!allSerials.Add(serial))
                        ModelState.AddModelError(string.Empty, $"Serial/IMEI {serial} bị trùng trong phiếu nhập.");
                }
                serialsByRow.Add(rowSerials);
            }

            if (allSerials.Count > 0)
            {
                var duplicateSerials = await _context.SerialSanPhams.AsNoTracking()
                    .Where(x => allSerials.Contains(x.MaSerial))
                    .Select(x => x.MaSerial)
                    .ToListAsync();
                foreach (var serial in duplicateSerials)
                    ModelState.AddModelError(string.Empty, $"Serial/IMEI {serial} đã tồn tại.");
            }

            if (!ModelState.IsValid)
            {
                await LoadSelections();
                return View(phieuNhap);
            }

            phieuNhap.TongTien = details.Sum(x => x.DonGiaNhap * x.SoLuong);

            await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);
            for (var index = 0; index < details.Count; index++)
            {
                var detail = details[index];
                phieuNhap.ChiTietPhieuNhaps.Add(detail);

                var product = products[detail.SanPhamId];
                product.SoLuongTon += detail.SoLuong;
                product.GiaNhap = detail.DonGiaNhap;
                product.NgayCapNhat = DateTime.Now;

                foreach (var serial in serialsByRow[index])
                {
                    detail.SerialSanPhams.Add(new SerialSanPham
                    {
                        MaSerial = serial,
                        SanPhamId = detail.SanPhamId,
                        TrangThai = TrangThaiSerial.TrongKho
                    });
                }
            }

            _context.PhieuNhaps.Add(phieuNhap);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            TempData["Success"] = $"Đã lưu {phieuNhap.MaPhieuNhap} và đồng bộ {allSerials.Count} serial/IMEI.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in Create POST");
            ModelState.AddModelError(string.Empty, $"Không thể lưu phiếu nhập: {ex.Message}");
            await LoadSelections();
            return View(phieuNhap);
        }
    }

    private async Task LoadSelections()
    {
        ViewBag.NhaCungCaps = await _context.NhaCungCaps.Where(x => x.TrangThai).OrderBy(x => x.TenNCC).ToListAsync();
        ViewBag.SanPhams = await _context.SanPhams.Where(x => x.TrangThai).OrderBy(x => x.TenSanPham).ToListAsync();
    }

    private static List<string> ParseSerials(string? value) =>
        string.IsNullOrWhiteSpace(value)
            ? []
            : value.Split(['\r', '\n', ',', ';'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(x => x.ToUpperInvariant())
                .ToList();
}

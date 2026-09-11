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
public class XuatKhoController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<XuatKhoController> _logger;

    public XuatKhoController(ApplicationDbContext context, ILogger<XuatKhoController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var vouchers = await _context.PhieuDieuChinhTonKhos
            .AsNoTracking()
            .Include(x => x.NguoiThucHien)
            .Include(x => x.ChiTiets)
            .OrderByDescending(x => x.NgayTao)
            .ToListAsync();
        return View(vouchers);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        await LoadProducts();
        return View(new TaoPhieuDieuChinhTonKhoVM
        {
            ChiTiets = [new TaoChiTietDieuChinhTonKhoVM { SoLuongThayDoi = 1 }]
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TaoPhieuDieuChinhTonKhoVM model)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue)
            return RedirectToAction("DangNhap", "TaiKhoan", new { area = "" });

        if (!Enum.IsDefined(model.LoaiGiaoDich))
            ModelState.AddModelError(nameof(model.LoaiGiaoDich), "Loại giao dịch không hợp lệ.");

        if (model.ChiTiets.Count != model.ChiTiets.Select(x => x.SanPhamId).Distinct().Count())
            ModelState.AddModelError(nameof(model.ChiTiets), "Mỗi sản phẩm chỉ được xuất hiện một lần trong phiếu.");

        if (!ModelState.IsValid)
        {
            await LoadProducts();
            return View(model);
        }

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
                await LoadProducts();
                return View(model);
            }

            var isIncrease = model.LoaiGiaoDich == LoaiGiaoDichTonKho.DieuChinhTang;
            var voucher = new PhieuDieuChinhTonKho
            {
                MaPhieu = $"TK-{DateTime.Now:yyyyMMddHHmmss}-{Guid.NewGuid():N}"[..33],
                NgayTao = DateTime.Now,
                NguoiThucHienId = userId.Value,
                LoaiGiaoDich = model.LoaiGiaoDich,
                LyDo = model.LyDo.Trim()
            };

            foreach (var item in model.ChiTiets)
            {
                var product = products[item.SanPhamId];
                var before = product.SoLuongTon;
                var after = isIncrease
                    ? checked(before + item.SoLuongThayDoi)
                    : before - item.SoLuongThayDoi;

                if (after < 0)
                {
                    ModelState.AddModelError(
                        nameof(model.ChiTiets),
                        $"Sản phẩm {product.MaSanPham} - {product.TenSanPham} chỉ còn {before}, không đủ để giảm {item.SoLuongThayDoi}.");
                    await transaction.RollbackAsync();
                    await LoadProducts();
                    return View(model);
                }

                product.SoLuongTon = after;
                product.NgayCapNhat = DateTime.Now;
                voucher.ChiTiets.Add(new ChiTietDieuChinhTonKho
                {
                    SanPhamId = product.Id,
                    SoLuongTruoc = before,
                    SoLuongThayDoi = item.SoLuongThayDoi,
                    SoLuongSau = after
                });
            }

            _context.PhieuDieuChinhTonKhos.Add(voucher);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            TempData["Success"] = $"Đã ghi nhận phiếu {voucher.MaPhieu}.";
            return RedirectToAction(nameof(Details), new { id = voucher.Id });
        }
        catch (OverflowException ex)
        {
            _logger.LogWarning(ex, "Stock adjustment quantity overflow");
            ModelState.AddModelError(nameof(model.ChiTiets), "Số lượng điều chỉnh vượt quá giới hạn cho phép.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unable to create stock issue/adjustment voucher");
            ModelState.AddModelError(string.Empty, "Không thể ghi nhận phiếu. Tồn kho chưa được thay đổi.");
        }

        await LoadProducts();
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var voucher = await _context.PhieuDieuChinhTonKhos
            .AsNoTracking()
            .Include(x => x.NguoiThucHien)
            .Include(x => x.ChiTiets)
            .ThenInclude(x => x.SanPham)
            .SingleOrDefaultAsync(x => x.Id == id);
        return voucher == null ? NotFound() : View(voucher);
    }

    private async Task LoadProducts()
    {
        ViewBag.SanPhams = await _context.SanPhams
            .AsNoTracking()
            .OrderBy(x => x.MaSanPham)
            .ToListAsync();
    }
}

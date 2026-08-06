using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VieITStore.Filters;
using VieITStore.Models;
using VieITStore.Models.Entities;
using VieITStore.Models.ViewModels;

namespace VieITStore.Areas.Admin.Controllers;

[Area("Admin")]
[AuthorizeRole(VaiTro.Admin, VaiTro.NhanVien)]
public class SerialSanPhamController : Controller
{
    private readonly ApplicationDbContext _context;

    public SerialSanPhamController(ApplicationDbContext context) => _context = context;

    public async Task<IActionResult> Index(string? search, TrangThaiSerial? trangThai)
    {
        var query = _context.SerialSanPhams.AsNoTracking().Include(x => x.SanPham).AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.Trim();
            query = query.Where(x => x.MaSerial.Contains(search)
                || x.SanPham!.MaSanPham.Contains(search)
                || x.SanPham.TenSanPham.Contains(search));
        }
        if (trangThai.HasValue) query = query.Where(x => x.TrangThai == trangThai);

        ViewBag.Search = search;
        ViewBag.TrangThai = trangThai;
        return View(await query.OrderByDescending(x => x.NgayTao).ToListAsync());
    }

    public async Task<IActionResult> Create()
    {
        await LoadSelections();
        return View(new SerialSanPhamVM());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SerialSanPhamVM model)
    {
        Normalize(model);
        if (model.ChiTietDonHangId.HasValue) model.TrangThai = TrangThaiSerial.DaBan;
        await ValidateModel(model, null, null);
        if (!ModelState.IsValid)
        {
            await LoadSelections(model);
            return View(model);
        }

        var entity = new SerialSanPham
        {
            MaSerial = model.MaSerial,
            SanPhamId = model.SanPhamId,
            ChiTietPhieuNhapId = model.ChiTietPhieuNhapId,
            ChiTietDonHangId = model.ChiTietDonHangId,
            TrangThai = model.ChiTietDonHangId.HasValue ? TrangThaiSerial.DaBan : model.TrangThai,
            GhiChu = model.GhiChu
        };
        await SetWarrantyDates(entity);
        _context.SerialSanPhams.Add(entity);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError(nameof(model.MaSerial), "Serial/IMEI đã tồn tại.");
            await LoadSelections(model);
            return View(model);
        }

        TempData["Success"] = "Đã thêm serial/IMEI.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var entity = await _context.SerialSanPhams.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null) return NotFound();
        var model = new SerialSanPhamVM
        {
            Id = entity.Id,
            MaSerial = entity.MaSerial,
            SanPhamId = entity.SanPhamId,
            ChiTietPhieuNhapId = entity.ChiTietPhieuNhapId,
            ChiTietDonHangId = entity.ChiTietDonHangId,
            TrangThai = entity.TrangThai,
            GhiChu = entity.GhiChu
        };
        await LoadSelections(model);
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, SerialSanPhamVM model)
    {
        if (id != model.Id) return NotFound();
        Normalize(model);
        var entity = await _context.SerialSanPhams.FindAsync(id);
        if (entity == null) return NotFound();

        if (!entity.ChiTietDonHangId.HasValue && model.ChiTietDonHangId.HasValue)
            model.TrangThai = TrangThaiSerial.DaBan;

        await ValidateModel(model, id, entity);
        if (!ModelState.IsValid)
        {
            await LoadSelections(model);
            return View(model);
        }

        entity.MaSerial = model.MaSerial;
        entity.SanPhamId = model.SanPhamId;
        entity.ChiTietPhieuNhapId = model.ChiTietPhieuNhapId;
        entity.ChiTietDonHangId = model.ChiTietDonHangId;
        entity.TrangThai = model.TrangThai;
        entity.GhiChu = model.GhiChu;
        await SetWarrantyDates(entity);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError(nameof(model.MaSerial), "Serial/IMEI đã tồn tại.");
            await LoadSelections(model);
            return View(model);
        }

        TempData["Success"] = "Đã cập nhật serial/IMEI.";
        return RedirectToAction(nameof(Details), new { id });
    }

    public async Task<IActionResult> Details(int id)
    {
        var entity = await _context.SerialSanPhams.AsNoTracking()
            .Include(x => x.SanPham)
            .Include(x => x.ChiTietPhieuNhap).ThenInclude(x => x!.PhieuNhap).ThenInclude(x => x!.NhaCungCap)
            .Include(x => x.ChiTietDonHang).ThenInclude(x => x!.DonHang).ThenInclude(x => x!.KhachHang).ThenInclude(x => x!.NguoiDung)
            .FirstOrDefaultAsync(x => x.Id == id);
        return entity == null ? NotFound() : View(entity);
    }

    private async Task ValidateModel(SerialSanPhamVM model, int? excludedId, SerialSanPham? existing)
    {
        if (await _context.SerialSanPhams.AnyAsync(x => x.MaSerial == model.MaSerial && (!excludedId.HasValue || x.Id != excludedId)))
            ModelState.AddModelError(nameof(model.MaSerial), "Serial/IMEI đã tồn tại.");

        if (!await _context.SanPhams.AnyAsync(x => x.Id == model.SanPhamId))
            ModelState.AddModelError(nameof(model.SanPhamId), "Sản phẩm không tồn tại.");

        if (existing != null && (existing.ChiTietPhieuNhapId.HasValue || existing.ChiTietDonHangId.HasValue)
            && existing.SanPhamId != model.SanPhamId)
            ModelState.AddModelError(nameof(model.SanPhamId), "Không thể đổi sản phẩm của serial đã phát sinh giao dịch.");

        if (model.ChiTietPhieuNhapId.HasValue)
        {
            var detail = await _context.ChiTietPhieuNhaps.AsNoTracking().FirstOrDefaultAsync(x => x.Id == model.ChiTietPhieuNhapId);
            if (detail == null || detail.SanPhamId != model.SanPhamId)
                ModelState.AddModelError(nameof(model.ChiTietPhieuNhapId), "Nguồn nhập không thuộc sản phẩm đã chọn.");
            else if (await _context.SerialSanPhams.CountAsync(x => x.ChiTietPhieuNhapId == detail.Id && (!excludedId.HasValue || x.Id != excludedId)) >= detail.SoLuong)
                ModelState.AddModelError(nameof(model.ChiTietPhieuNhapId), "Số serial đã gán đã đủ số lượng của dòng nhập.");
        }

        if (existing?.ChiTietDonHangId.HasValue == true && existing.ChiTietDonHangId != model.ChiTietDonHangId)
            ModelState.AddModelError(nameof(model.ChiTietDonHangId), "Serial đã bán không thể chuyển sang đơn hàng khác hoặc bỏ liên kết bán.");

        if (model.ChiTietDonHangId.HasValue)
        {
            var detail = await _context.ChiTietDonHangs.AsNoTracking().Include(x => x.DonHang)
                .FirstOrDefaultAsync(x => x.Id == model.ChiTietDonHangId);
            if (detail == null || detail.SanPhamId != model.SanPhamId || detail.DonHang?.TrangThai == TrangThaiDonHang.DaHuy)
                ModelState.AddModelError(nameof(model.ChiTietDonHangId), "Dòng bán không hợp lệ hoặc không thuộc sản phẩm đã chọn.");
            else if (await _context.SerialSanPhams.CountAsync(x => x.ChiTietDonHangId == detail.Id && (!excludedId.HasValue || x.Id != excludedId)) >= detail.SoLuong)
                ModelState.AddModelError(nameof(model.ChiTietDonHangId), "Số serial đã gán đã đủ số lượng của dòng đơn hàng.");

            if (model.TrangThai == TrangThaiSerial.TrongKho)
                ModelState.AddModelError(nameof(model.TrangThai), "Serial đã bán không thể ở trạng thái Trong kho.");
        }
        else if (model.TrangThai is TrangThaiSerial.DaBan or TrangThaiSerial.DangBaoHanh)
        {
            ModelState.AddModelError(nameof(model.TrangThai), "Trạng thái này yêu cầu liên kết với một chi tiết đơn hàng.");
        }
    }

    private async Task SetWarrantyDates(SerialSanPham entity)
    {
        if (!entity.ChiTietDonHangId.HasValue)
        {
            entity.NgayBatDauBaoHanh = null;
            entity.NgayHetHanBaoHanh = null;
            return;
        }

        var sale = await _context.ChiTietDonHangs.AsNoTracking()
            .Include(x => x.DonHang).Include(x => x.SanPham)
            .FirstAsync(x => x.Id == entity.ChiTietDonHangId.Value);
        var start = sale.DonHang?.NgayThanhToan ?? sale.DonHang?.NgayDat ?? DateTime.Now;
        entity.NgayBatDauBaoHanh = start;
        entity.NgayHetHanBaoHanh = start.AddMonths(Math.Max(0, sale.SanPham?.BaoHanh ?? 0));
    }

    private async Task LoadSelections(SerialSanPhamVM? model = null)
    {
        var selectedReceiptDetailId = model?.ChiTietPhieuNhapId;
        var selectedOrderDetailId = model?.ChiTietDonHangId;
        ViewBag.SanPhams = new SelectList(await _context.SanPhams.AsNoTracking().OrderBy(x => x.TenSanPham).ToListAsync(), "Id", "TenSanPham", model?.SanPhamId);
        ViewBag.PhieuNhaps = await _context.ChiTietPhieuNhaps.AsNoTracking()
            .Include(x => x.PhieuNhap).Include(x => x.SanPham)
            .OrderByDescending(x => x.PhieuNhap!.NgayNhap)
            .Select(x => new SelectListItem($"{x.PhieuNhap!.MaPhieuNhap} - {x.SanPham!.MaSanPham} ({x.SoLuong})", x.Id.ToString(), x.Id == selectedReceiptDetailId))
            .ToListAsync();
        ViewBag.DonHangs = await _context.ChiTietDonHangs.AsNoTracking()
            .Include(x => x.DonHang).Include(x => x.SanPham)
            .Where(x => x.DonHang!.TrangThai != TrangThaiDonHang.DaHuy)
            .OrderByDescending(x => x.DonHang!.NgayDat)
            .Select(x => new SelectListItem($"{x.DonHang!.MaDonHang} - {x.SanPham!.MaSanPham} ({x.SoLuong})", x.Id.ToString(), x.Id == selectedOrderDetailId))
            .ToListAsync();
    }

    private static void Normalize(SerialSanPhamVM model)
    {
        model.MaSerial = model.MaSerial?.Trim().ToUpperInvariant() ?? string.Empty;
        model.GhiChu = string.IsNullOrWhiteSpace(model.GhiChu) ? null : model.GhiChu.Trim();
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
using VieITStore.Filters;
using VieITStore.Models;
using VieITStore.Models.Entities;
using VieITStore.Models.ViewModels;

namespace VieITStore.Areas.Admin.Controllers;

[Area("Admin")]
[AuthorizeRole(VaiTro.Admin)]
public class BaoCaoController : Controller
{
    private readonly ApplicationDbContext _context;

    public BaoCaoController(ApplicationDbContext context) => _context = context;

    [HttpGet]
    public async Task<IActionResult> Index(DateTime? tuNgay, DateTime? denNgay)
    {
        var (from, to) = NormalizeRange(tuNgay, denNgay);
        if (from > to)
        {
            ModelState.AddModelError(string.Empty, "Từ ngày không được sau đến ngày.");
            (from, to) = (to, from);
        }

        return View(await BuildReport(from, to));
    }

    [HttpGet]
    public async Task<IActionResult> ExportCsv(DateTime? tuNgay, DateTime? denNgay)
    {
        var (from, to) = NormalizeRange(tuNgay, denNgay);
        if (from > to) (from, to) = (to, from);
        var report = await BuildReport(from, to);

        var csv = new StringBuilder();
        csv.AppendLine("BÁO CÁO KINH DOANH VIEITSTORE");
        csv.AppendLine($"Từ ngày;{from:dd/MM/yyyy}");
        csv.AppendLine($"Đến ngày;{to:dd/MM/yyyy}");
        csv.AppendLine($"Doanh thu hoàn thành;{report.DoanhThu:0}");
        csv.AppendLine($"Tổng đơn;{report.TongDon}");
        csv.AppendLine($"Giá trị nhập kho;{report.GiaTriNhap:0}");
        csv.AppendLine();
        csv.AppendLine("DOANH THU THEO NGÀY");
        csv.AppendLine("Ngày;Số đơn;Doanh thu");
        foreach (var row in report.DoanhThuTheoNgay)
            csv.AppendLine($"{row.Ngay:dd/MM/yyyy};{row.SoDon};{row.DoanhThu:0}");

        csv.AppendLine();
        csv.AppendLine("SẢN PHẨM BÁN CHẠY");
        csv.AppendLine("Mã sản phẩm;Tên sản phẩm;Số lượng;Doanh thu");
        foreach (var row in report.BanChay)
            csv.AppendLine($"{Csv(row.MaSanPham)};{Csv(row.TenSanPham)};{row.SoLuong};{row.DoanhThu:0}");

        csv.AppendLine();
        csv.AppendLine("TỒN KHO THẤP");
        csv.AppendLine("Mã sản phẩm;Tên sản phẩm;Số lượng tồn");
        foreach (var row in report.TonKhoThap)
            csv.AppendLine($"{Csv(row.MaSanPham)};{Csv(row.TenSanPham)};{row.SoLuongTon}");

        var encoding = new UTF8Encoding(true);
        var content = encoding.GetPreamble().Concat(encoding.GetBytes(csv.ToString())).ToArray();
        return File(content, "text/csv; charset=utf-8", $"bao-cao-{from:yyyyMMdd}-{to:yyyyMMdd}.csv");
    }

    private async Task<BaoCaoVM> BuildReport(DateTime from, DateTime to)
    {
        var end = to < DateTime.MaxValue.Date ? to.AddDays(1) : DateTime.MaxValue;
        var orderRows = await _context.DonHangs.AsNoTracking()
            .Where(x => x.NgayDat >= from && x.NgayDat < end)
            .Select(x => new { x.Id, x.NgayDat, x.TrangThai, x.TongThanhToan })
            .ToListAsync();
        var completedIds = orderRows.Where(x => x.TrangThai == TrangThaiDonHang.HoanThanh).Select(x => x.Id).ToList();
        var sales = completedIds.Count == 0
            ? []
            : await _context.ChiTietDonHangs.AsNoTracking().Include(x => x.SanPham)
                .Where(x => completedIds.Contains(x.DonHangId)).ToListAsync();

        return new BaoCaoVM
        {
            TuNgay = from,
            DenNgay = to,
            DoanhThu = orderRows.Where(x => x.TrangThai == TrangThaiDonHang.HoanThanh).Sum(x => x.TongThanhToan),
            TongDon = orderRows.Count,
            GiaTriNhap = await _context.PhieuNhaps.AsNoTracking()
                .Where(x => x.NgayNhap >= from && x.NgayNhap < end)
                .SumAsync(x => (decimal?)x.TongTien) ?? 0,
            DoanhThuTheoNgay = orderRows.Where(x => x.TrangThai == TrangThaiDonHang.HoanThanh)
                .GroupBy(x => x.NgayDat.Date)
                .Select(g => new BaoCaoTheoNgayVM(g.Key, g.Sum(x => x.TongThanhToan), g.Count()))
                .OrderBy(x => x.Ngay).ToList(),
            DonTheoTrangThai = orderRows.GroupBy(x => x.TrangThai)
                .Select(g => new BaoCaoTrangThaiVM(g.Key, g.Count())).ToList(),
            BanChay = sales.Where(x => x.SanPham != null)
                .GroupBy(x => new { x.SanPham!.MaSanPham, x.SanPham.TenSanPham })
                .Select(g => new BaoCaoSanPhamVM(g.Key.MaSanPham, g.Key.TenSanPham, g.Sum(x => x.SoLuong), g.Sum(x => x.ThanhTien)))
                .OrderByDescending(x => x.SoLuong).Take(10).ToList(),
            TonKhoThap = await _context.SanPhams.AsNoTracking()
                .Where(x => x.TrangThai && x.SoLuongTon <= 10)
                .OrderBy(x => x.SoLuongTon)
                .Select(x => new BaoCaoTonKhoVM(x.MaSanPham, x.TenSanPham, x.SoLuongTon))
                .Take(10).ToListAsync()
        };
    }

    private static (DateTime From, DateTime To) NormalizeRange(DateTime? from, DateTime? to)
    {
        var today = DateTime.Today;
        return ((from ?? new DateTime(today.Year, today.Month, 1)).Date, (to ?? today).Date);
    }

    private static string Csv(string value)
    {
        value = value.Trim();
        if (value.Length > 0 && "=+-@".Contains(value[0])) value = "'" + value;
        return $"\"{value.Replace("\"", "\"\"")}\"";
    }
}

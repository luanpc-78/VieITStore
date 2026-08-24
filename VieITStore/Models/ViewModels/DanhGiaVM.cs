using System.ComponentModel.DataAnnotations;
using VieITStore.Models.Entities;

namespace VieITStore.Models.ViewModels;
public class DanhGiaFormVM { public int SanPhamId { get; set; } [Range(1,5)] public int SoSao { get; set; } [Required, StringLength(1000, MinimumLength=10)] public string NoiDung { get; set; } = string.Empty; }
public class DanhGiaSanPhamVM { public double DiemTrungBinh { get; set; } public int TongDanhGia { get; set; } public bool DuocDanhGia { get; set; } public DanhGia? DanhGiaCuaToi { get; set; } public List<DanhGia> DanhGias { get; set; } = []; }

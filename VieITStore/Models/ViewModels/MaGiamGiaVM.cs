using System.ComponentModel.DataAnnotations;

namespace VieITStore.Models.ViewModels
{
    public class MaGiamGiaVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Mã code không được để trống")]
        [MaxLength(50)]
        public string MaCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tên khuyến mại không được để trống")]
        [MaxLength(255)]
        public string TenKhuyenMai { get; set; } = string.Empty;

        public string? MoTa { get; set; }

        [Required(ErrorMessage = "Phần trăm giảm không được để trống")]
        [Range(0.01, 100, ErrorMessage = "Phần trăm giảm phải từ 0.01 đến 100")]
        public decimal PhanTramGiam { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Giá giảm tối đa phải lớn hơn 0")]
        public decimal? GiaGiamToiDa { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Giá tối thiểu phải lớn hơn 0")]
        public decimal? GiaToiThieu { get; set; }

        [Required(ErrorMessage = "Ngày bắt đầu không được để trống")]
        public DateTime NgayBatDau { get; set; }

        [Required(ErrorMessage = "Ngày kết thúc không được để trống")]
        public DateTime NgayKetThuc { get; set; }

        public int? SoLuongSuDung { get; set; }

        public bool ApDungToanBo { get; set; } = true;

        public int? DanhMucId { get; set; }

        public bool TrangThai { get; set; } = true;
    }
}

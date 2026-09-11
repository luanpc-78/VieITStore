using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VieITStore.Models.Entities
{
    public class HoaDon
    {
        [Key]
        public int Id { get; set; }

        public int? DonHangId { get; set; }
        public virtual DonHang? DonHang { get; set; }

        [Required]
        [MaxLength(50)]
        [Display(Name = "Số hóa đơn")]
        public string SoHoaDon { get; set; } = string.Empty;

        [Display(Name = "Ngày lập")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime NgayLap { get; set; } = DateTime.Now;

        [Required]
        [Display(Name = "Khách hàng")]
        public int KhachHangId { get; set; }
        public virtual KhachHang? KhachHang { get; set; }

        [Display(Name = "Người lập")]
        public string NguoiLap { get; set; } = string.Empty;

        [Display(Name = "Tổng tiền hàng")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TongTienHang { get; set; }

        [Display(Name = "Thuế VAT (%)")]
        public decimal ThueVAT { get; set; } = 10; // Mặc định 10%

        [Display(Name = "Tiền thuế VAT")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TienThueVAT { get; set; }

        [Display(Name = "Tổng thanh toán")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TongThanhToan { get; set; }

        [Display(Name = "Giảm giá (VNĐ)")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal GiamGia { get; set; }

        [Display(Name = "Thành tiền")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal ThanhTien { get; set; }

        [Display(Name = "Phương thức TT")]
        public string PhuongThucThanhToan { get; set; } = "Tiền mặt"; // Tiền mặt / Chuyển khoản / Thẻ

        [Display(Name = "Ghi chú")]
        public string? GhiChu { get; set; }

        [Display(Name = "Trạng thái")]
        public string TrangThai { get; set; } = "Hoàn thành"; // Hoàn thành / Đang xử lý / Hủy

        public virtual ICollection<ChiTietHoaDon> ChiTietHoaDons { get; set; } = new List<ChiTietHoaDon>();
    }
}

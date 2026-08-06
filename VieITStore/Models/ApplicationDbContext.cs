using Microsoft.EntityFrameworkCore;
using VieITStore.Models.Entities;

namespace VieITStore.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<DanhMuc> DanhMucs { get; set; }
        public DbSet<SanPham> SanPhams { get; set; }
        public DbSet<NguoiDung> NguoiDungs { get; set; }
        public DbSet<KhachHang> KhachHangs { get; set; }
        public DbSet<DiaChiGiaoHang> DiaChiGiaoHangs { get; set; }
        public DbSet<GioHang> GioHangs { get; set; }
        public DbSet<DonHang> DonHangs { get; set; }
        public DbSet<ChiTietDonHang> ChiTietDonHangs { get; set; }
        public DbSet<HoaDon> HoaDons { get; set; }
        public DbSet<ChiTietHoaDon> ChiTietHoaDons { get; set; }
        public DbSet<NhaCungCap> NhaCungCaps { get; set; }
        public DbSet<PhieuNhap> PhieuNhaps { get; set; }
        public DbSet<ChiTietPhieuNhap> ChiTietPhieuNhaps { get; set; }
        public DbSet<DanhGia> DanhGias { get; set; }
        public DbSet<PhieuDieuChinhTonKho> PhieuDieuChinhTonKhos { get; set; }
        public DbSet<ChiTietDieuChinhTonKho> ChiTietDieuChinhTonKhos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<NhaCungCap>()
                .HasIndex(x => x.MaNCC)
                .IsUnique();

            modelBuilder.Entity<PhieuDieuChinhTonKho>()
                .HasIndex(x => x.MaPhieu)
                .IsUnique();

            modelBuilder.Entity<PhieuDieuChinhTonKho>()
                .HasOne(x => x.NguoiThucHien)
                .WithMany()
                .HasForeignKey(x => x.NguoiThucHienId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ChiTietDieuChinhTonKho>()
                .HasOne(x => x.SanPham)
                .WithMany()
                .HasForeignKey(x => x.SanPhamId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DonHang>()
                .HasIndex(x => x.MaDonHang)
                .IsUnique();

            modelBuilder.Entity<HoaDon>()
                .HasIndex(h => h.DonHangId)
                .IsUnique();

            modelBuilder.Entity<HoaDon>()
                .HasOne(h => h.DonHang)
                .WithOne(d => d.HoaDon)
                .HasForeignKey<HoaDon>(h => h.DonHangId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<HoaDon>()
                .HasIndex(h => h.SoHoaDon)
                .IsUnique();

            modelBuilder.Entity<HoaDon>()
                .Property(h => h.ThueVAT)
                .HasPrecision(5, 2);

            // Seed DanhMuc
            modelBuilder.Entity<DanhMuc>().HasData(
                new DanhMuc { Id = 1, TenDanhMuc = "Linh kiện điện tử", ThuTu = 1 },
                new DanhMuc { Id = 2, TenDanhMuc = "Phụ kiện điện thoại", ThuTu = 2 },
                new DanhMuc { Id = 3, TenDanhMuc = "Phụ kiện máy tính", ThuTu = 3 },
                new DanhMuc { Id = 4, TenDanhMuc = "Thiết bị điện tử", ThuTu = 4 }
            );

            // Seed NguoiDung (Admin + NhanVien)
            modelBuilder.Entity<NguoiDung>().HasData(
                new NguoiDung
                {
                    Id = 1,
                    TenDangNhap = "admin",
                    MatKhauHash = "admin123", // Plain text for testing - change to BCrypt hash in production
                    HoTen = "Quản trị viên",
                    Email = "admin@vieitstore.com",
                    VaiTro = VaiTro.Admin,
                    TrangThai = true,
                    NgayTao = DateTime.Now
                },
                new NguoiDung
                {
                    Id = 2,
                    TenDangNhap = "nhanvien",
                    MatKhauHash = "nhanvien123", // Plain text for testing
                    HoTen = "Nhân viên bán hàng",
                    Email = "nhanvien@vieitstore.com",
                    VaiTro = VaiTro.NhanVien,
                    TrangThai = true,
                    NgayTao = DateTime.Now
                },
                new NguoiDung
                {
                    Id = 3,
                    TenDangNhap = "khachhang",
                    MatKhauHash = "khachhang123", // Plain text for testing
                    HoTen = "Nguyễn Văn A",
                    Email = "khachhang@vieitstore.com",
                    VaiTro = VaiTro.KhachHang,
                    TrangThai = true,
                    NgayTao = DateTime.Now
                }
            );

            // Seed NhaCungCap
            modelBuilder.Entity<NhaCungCap>().HasData(
                new NhaCungCap { Id = 1, MaNCC = "NCC001", TenNCC = "Công ty TNHH Điện tử ABC", NguoiLienHe = "Nguyễn Văn A", SoDienThoai = "0909123456", DiaChi = "Quận 1, TP.HCM" },
                new NhaCungCap { Id = 2, MaNCC = "NCC002", TenNCC = "Phân phối Linh kiện XYZ", NguoiLienHe = "Trần Thị B", SoDienThoai = "0912345678", DiaChi = "Quận 3, TP.HCM" }
            );

            // Seed KhachHang
            modelBuilder.Entity<KhachHang>().HasData(
                new KhachHang
                {
                    Id = 1,
                    NguoiDungId = 3,
                    GioiTinh = "Nam",
                    NgaySinh = new DateTime(1990, 5, 15)
                }
            );

            // Seed SanPham
            modelBuilder.Entity<SanPham>().HasData(
                new SanPham
                {
                    Id = 1,
                    TenSanPham = "CPU Intel Core i5-12400F",
                    MaSanPham = "CPU001",
                    DanhMucId = 1,
                    MoTaNgan = "Bộ xử lý Intel Core i5 generasi 12",
                    GiaNhap = 2500000,
                    GiaBan = 3990000,
                    GiaKhuyenMai = 3590000,
                    HinhAnh = "/images/cpu-i5.jpg",
                    SoLuongTon = 50,
                    SoLuongDaBan = 10,
                    TrangThai = true
                },
                new SanPham
                {
                    Id = 2,
                    TenSanPham = "VGA ASUS RTX 3060 12GB",
                    MaSanPham = "GPU001",
                    DanhMucId = 1,
                    MoTaNgan = "Card đồ họa NVIDIA RTX 3060 12GB GDDR6",
                    GiaNhap = 5000000,
                    GiaBan = 7990000,
                    GiaKhuyenMai = 7490000,
                    HinhAnh = "/images/gpu-rtx3060.jpg",
                    SoLuongTon = 30,
                    SoLuongDaBan = 5,
                    TrangThai = true
                },
                new SanPham
                {
                    Id = 3,
                    TenSanPham = "SSD Samsung 980 NVMe 500GB",
                    MaSanPham = "SSD001",
                    DanhMucId = 1,
                    MoTaNgan = "Ổ cứng NVMe tốc độ cao 500GB",
                    GiaNhap = 600000,
                    GiaBan = 990000,
                    GiaKhuyenMai = 890000,
                    HinhAnh = "/images/ssd-samsung.jpg",
                    SoLuongTon = 100,
                    SoLuongDaBan = 25,
                    TrangThai = true
                },
                new SanPham
                {
                    Id = 4,
                    TenSanPham = "Cáp sạc nhanh Type-C to Lightning 20W",
                    MaSanPham = "ACC001",
                    DanhMucId = 2,
                    MoTaNgan = "Cáp sạc nhanh Type-C to Lightning chất lượng cao",
                    GiaNhap = 80000,
                    GiaBan = 150000,
                    GiaKhuyenMai = 129000,
                    HinhAnh = "/images/cable-typec.jpg",
                    SoLuongTon = 200,
                    SoLuongDaBan = 60,
                    TrangThai = true
                },
                new SanPham
                {
                    Id = 5,
                    TenSanPham = "Chuột Logitech G502 HERO",
                    MaSanPham = "ACC002",
                    DanhMucId = 3,
                    MoTaNgan = "Chuột gaming Logitech G502 HERO chuyên nghiệp",
                    GiaNhap = 600000,
                    GiaBan = 899000,
                    GiaKhuyenMai = null,
                    HinhAnh = "/images/mouse-logitech.jpg",
                    SoLuongTon = 45,
                    SoLuongDaBan = 15,
                    TrangThai = true
                }
            );
        }
    }
}

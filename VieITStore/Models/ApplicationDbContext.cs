using Microsoft.EntityFrameworkCore;
using VieITStore.Models.Entities;

namespace VieITStore.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<DanhMuc> DanhMucs { get; set; }
        public DbSet<SanPham> SanPhams { get; set; }
        public DbSet<Mau> Maus { get; set; }
        public DbSet<MaGiamGia> MaGiamGias { get; set; }
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
        public DbSet<SerialSanPham> SerialSanPhams { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<NguoiDung>()
                .HasIndex(x => x.Email)
                .IsUnique();

            modelBuilder.Entity<NhaCungCap>()
                .HasIndex(x => x.MaNCC)
                .IsUnique();

            modelBuilder.Entity<SerialSanPham>()
                .HasIndex(x => x.MaSerial)
                .IsUnique();

            modelBuilder.Entity<SanPham>()
                .HasIndex(x => x.MaSanPham)
                .IsUnique();

            modelBuilder.Entity<SanPham>()
                .HasIndex(x => x.MaVach)
                .IsUnique()
                .HasFilter("[MaVach] IS NOT NULL");

            modelBuilder.Entity<GioHang>()
                .HasIndex(x => new { x.NguoiDungId, x.SanPhamId })
                .IsUnique()
                .HasFilter("[NguoiDungId] IS NOT NULL");

            modelBuilder.Entity<GioHang>()
                .HasIndex(x => new { x.SessionId, x.SanPhamId })
                .IsUnique()
                .HasFilter("[SessionId] IS NOT NULL");

            modelBuilder.Entity<GioHang>()
                .ToTable(t => t.HasCheckConstraint(
                    "CK_GioHangs_Owner",
                    "([NguoiDungId] IS NOT NULL AND [SessionId] IS NULL) OR ([NguoiDungId] IS NULL AND [SessionId] IS NOT NULL)"));

            modelBuilder.Entity<GioHang>()
                .HasOne(x => x.NguoiDung)
                .WithMany()
                .HasForeignKey(x => x.NguoiDungId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<GioHang>()
                .HasOne(x => x.SanPham)
                .WithMany()
                .HasForeignKey(x => x.SanPhamId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SerialSanPham>()
                .HasOne(x => x.SanPham)
                .WithMany(x => x.SerialSanPhams)
                .HasForeignKey(x => x.SanPhamId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SerialSanPham>()
                .HasOne(x => x.ChiTietPhieuNhap)
                .WithMany(x => x.SerialSanPhams)
                .HasForeignKey(x => x.ChiTietPhieuNhapId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SerialSanPham>()
                .HasOne(x => x.ChiTietDonHang)
                .WithMany(x => x.SerialSanPhams)
                .HasForeignKey(x => x.ChiTietDonHangId)
                .OnDelete(DeleteBehavior.Restrict);

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

            modelBuilder.Entity<DonHang>()
                .HasOne(x => x.NguoiCapNhatTrangThai)
                .WithMany()
                .HasForeignKey(x => x.NguoiCapNhatTrangThaiId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DanhGia>()
                .HasIndex(x => new { x.KhachHangId, x.SanPhamId })
                .IsUnique();

            modelBuilder.Entity<DanhGia>()
                .HasOne(x => x.KhachHang)
                .WithMany(x => x.DanhGias)
                .HasForeignKey(x => x.KhachHangId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DanhGia>()
                .HasOne(x => x.SanPham)
                .WithMany(x => x.DanhGias)
                .HasForeignKey(x => x.SanPhamId)
                .OnDelete(DeleteBehavior.Restrict);

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
                    MatKhauHash = "$2a$11$JyJ7cHu9cnJSM4k/dtsifORQDaZSUWn7cuFHCeeOu4pjezwpUarqm",
                    HoTen = "Quản trị viên",
                    Email = "admin@vieitstore.com",
                    VaiTro = VaiTro.Admin,
                    TrangThai = true,
                    NgayTao = new DateTime(2026, 8, 6, 19, 43, 59, 742, DateTimeKind.Local).AddTicks(1862)
                },
                new NguoiDung
                {
                    Id = 2,
                    TenDangNhap = "nhanvien",
                    MatKhauHash = "$2a$11$BC0g0F3PFYN0.wmPKGVH2eZyOT.K6tkmPZFTScnWbsqJOPl9../3y",
                    HoTen = "Nhân viên bán hàng",
                    Email = "nhanvien@vieitstore.com",
                    VaiTro = VaiTro.NhanVien,
                    TrangThai = true,
                    NgayTao = new DateTime(2026, 8, 6, 19, 43, 59, 742, DateTimeKind.Local).AddTicks(2507)
                },
                new NguoiDung
                {
                    Id = 3,
                    TenDangNhap = "khachhang",
                    MatKhauHash = "$2a$11$ub58BEpDCXROdMsUtf7nnui5Xj91n63eWJ16pDIvnKM2pIBdql2Mi",
                    HoTen = "Nguyễn Văn A",
                    Email = "khachhang@vieitstore.com",
                    VaiTro = VaiTro.KhachHang,
                    TrangThai = true,
                    NgayTao = new DateTime(2026, 8, 6, 19, 43, 59, 742, DateTimeKind.Local).AddTicks(2510)
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
                    NoiBat = true,
                    NgayTao = new DateTime(2026, 8, 6, 19, 43, 59, 743, DateTimeKind.Local).AddTicks(5978),
                    NgayCapNhat = new DateTime(2026, 8, 6, 19, 43, 59, 743, DateTimeKind.Local).AddTicks(5983),
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
                    NoiBat = true,
                    NgayTao = new DateTime(2026, 8, 6, 19, 43, 59, 744, DateTimeKind.Local).AddTicks(3896),
                    NgayCapNhat = new DateTime(2026, 8, 6, 19, 43, 59, 744, DateTimeKind.Local).AddTicks(3899),
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
                    NoiBat = true,
                    NgayTao = new DateTime(2026, 8, 6, 19, 43, 59, 744, DateTimeKind.Local).AddTicks(3908),
                    NgayCapNhat = new DateTime(2026, 8, 6, 19, 43, 59, 744, DateTimeKind.Local).AddTicks(3908),
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
                    NgayTao = new DateTime(2026, 8, 6, 19, 43, 59, 744, DateTimeKind.Local).AddTicks(3913),
                    NgayCapNhat = new DateTime(2026, 8, 6, 19, 43, 59, 744, DateTimeKind.Local).AddTicks(3913),
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
                    NgayTao = new DateTime(2026, 8, 6, 19, 43, 59, 744, DateTimeKind.Local).AddTicks(3918),
                    NgayCapNhat = new DateTime(2026, 8, 6, 19, 43, 59, 744, DateTimeKind.Local).AddTicks(3919),
                    TrangThai = true
                }
            );
        }
    }
}

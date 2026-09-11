using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VieITStore.Migrations
{
    /// <inheritdoc />
    public partial class RepairBaselineTestData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                SET NOCOUNT ON;

                IF EXISTS (SELECT 1 FROM [NguoiDungs] WHERE LOWER(LTRIM(RTRIM([TenDangNhap]))) = N'admin')
                    UPDATE [NguoiDungs]
                    SET [TenDangNhap] = N'admin',
                        [MatKhauHash] = N'$2a$11$JyJ7cHu9cnJSM4k/dtsifORQDaZSUWn7cuFHCeeOu4pjezwpUarqm',
                        [HoTen] = CASE WHEN NULLIF(LTRIM(RTRIM([HoTen])), N'') IS NULL THEN N'Quản trị viên' ELSE [HoTen] END,
                        [VaiTro] = 1, [TrangThai] = 1
                    WHERE LOWER(LTRIM(RTRIM([TenDangNhap]))) = N'admin';
                ELSE
                    INSERT INTO [NguoiDungs] ([TenDangNhap], [MatKhauHash], [HoTen], [Email], [VaiTro], [TrangThai], [NgayTao])
                    VALUES (N'admin', N'$2a$11$JyJ7cHu9cnJSM4k/dtsifORQDaZSUWn7cuFHCeeOu4pjezwpUarqm', N'Quản trị viên', N'admin@vieitstore.com', 1, 1, GETDATE());

                IF EXISTS (SELECT 1 FROM [NguoiDungs] WHERE LOWER(LTRIM(RTRIM([TenDangNhap]))) = N'nhanvien')
                    UPDATE [NguoiDungs]
                    SET [TenDangNhap] = N'nhanvien',
                        [MatKhauHash] = N'$2a$11$BC0g0F3PFYN0.wmPKGVH2eZyOT.K6tkmPZFTScnWbsqJOPl9../3y',
                        [HoTen] = CASE WHEN NULLIF(LTRIM(RTRIM([HoTen])), N'') IS NULL THEN N'Nhân viên bán hàng' ELSE [HoTen] END,
                        [VaiTro] = 2, [TrangThai] = 1
                    WHERE LOWER(LTRIM(RTRIM([TenDangNhap]))) = N'nhanvien';
                ELSE
                    INSERT INTO [NguoiDungs] ([TenDangNhap], [MatKhauHash], [HoTen], [Email], [VaiTro], [TrangThai], [NgayTao])
                    VALUES (N'nhanvien', N'$2a$11$BC0g0F3PFYN0.wmPKGVH2eZyOT.K6tkmPZFTScnWbsqJOPl9../3y', N'Nhân viên bán hàng', N'nhanvien@vieitstore.com', 2, 1, GETDATE());

                IF EXISTS (SELECT 1 FROM [NguoiDungs] WHERE LOWER(LTRIM(RTRIM([TenDangNhap]))) = N'khachhang')
                    UPDATE [NguoiDungs]
                    SET [TenDangNhap] = N'khachhang',
                        [MatKhauHash] = N'$2a$11$ub58BEpDCXROdMsUtf7nnui5Xj91n63eWJ16pDIvnKM2pIBdql2Mi',
                        [HoTen] = CASE WHEN NULLIF(LTRIM(RTRIM([HoTen])), N'') IS NULL THEN N'Nguyễn Văn A' ELSE [HoTen] END,
                        [VaiTro] = 3, [TrangThai] = 1
                    WHERE LOWER(LTRIM(RTRIM([TenDangNhap]))) = N'khachhang';
                ELSE
                    INSERT INTO [NguoiDungs] ([TenDangNhap], [MatKhauHash], [HoTen], [Email], [VaiTro], [TrangThai], [NgayTao])
                    VALUES (N'khachhang', N'$2a$11$ub58BEpDCXROdMsUtf7nnui5Xj91n63eWJ16pDIvnKM2pIBdql2Mi', N'Nguyễn Văn A', N'khachhang@vieitstore.com', 3, 1, GETDATE());

                DECLARE @KhachHangUserId int = (
                    SELECT TOP (1) [Id] FROM [NguoiDungs]
                    WHERE LOWER(LTRIM(RTRIM([TenDangNhap]))) = N'khachhang' ORDER BY [Id]);
                IF @KhachHangUserId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM [KhachHangs] WHERE [NguoiDungId] = @KhachHangUserId)
                    INSERT INTO [KhachHangs] ([NguoiDungId], [GioiTinh], [NgaySinh])
                    VALUES (@KhachHangUserId, N'Nam', '1990-05-15');

                IF NOT EXISTS (SELECT 1 FROM [DanhMucs] WHERE [TenDanhMuc] = N'Linh kiện điện tử')
                    INSERT INTO [DanhMucs] ([TenDanhMuc], [ThuTu], [HienThi]) VALUES (N'Linh kiện điện tử', 1, 1);
                IF NOT EXISTS (SELECT 1 FROM [DanhMucs] WHERE [TenDanhMuc] = N'Phụ kiện điện thoại')
                    INSERT INTO [DanhMucs] ([TenDanhMuc], [ThuTu], [HienThi]) VALUES (N'Phụ kiện điện thoại', 2, 1);
                IF NOT EXISTS (SELECT 1 FROM [DanhMucs] WHERE [TenDanhMuc] = N'Phụ kiện máy tính')
                    INSERT INTO [DanhMucs] ([TenDanhMuc], [ThuTu], [HienThi]) VALUES (N'Phụ kiện máy tính', 3, 1);
                IF NOT EXISTS (SELECT 1 FROM [DanhMucs] WHERE [TenDanhMuc] = N'Thiết bị điện tử')
                    INSERT INTO [DanhMucs] ([TenDanhMuc], [ThuTu], [HienThi]) VALUES (N'Thiết bị điện tử', 4, 1);

                UPDATE [DanhMucs] SET [HienThi] = 1
                WHERE [TenDanhMuc] IN (N'Linh kiện điện tử', N'Phụ kiện điện thoại', N'Phụ kiện máy tính', N'Thiết bị điện tử');

                DECLARE @LinhKienId int = (SELECT TOP (1) [Id] FROM [DanhMucs] WHERE [TenDanhMuc] = N'Linh kiện điện tử' ORDER BY [Id]);
                DECLARE @PhuKienDienThoaiId int = (SELECT TOP (1) [Id] FROM [DanhMucs] WHERE [TenDanhMuc] = N'Phụ kiện điện thoại' ORDER BY [Id]);
                DECLARE @PhuKienMayTinhId int = (SELECT TOP (1) [Id] FROM [DanhMucs] WHERE [TenDanhMuc] = N'Phụ kiện máy tính' ORDER BY [Id]);

                IF NOT EXISTS (SELECT 1 FROM [SanPhams] WHERE [MaSanPham] = N'CPU001')
                    INSERT INTO [SanPhams] ([MaSanPham], [TenSanPham], [Slug], [MoTaNgan], [DanhMucId], [ThuongHieu], [GiaNhap], [GiaBan], [GiaKhuyenMai], [SoLuongTon], [DonViTinh], [HinhAnh], [BaoHanh], [XuatXu], [SoLuongDaBan], [NoiBat], [TrangThai], [NgayTao], [NgayCapNhat])
                    VALUES (N'CPU001', N'CPU Intel Core i5-12400F', N'cpu-intel-core-i5-12400f', N'Bộ xử lý Intel Core i5 thế hệ 12', @LinhKienId, N'Intel', 2500000, 3990000, 3590000, 50, N'Cái', N'/images/cpu-i5.jpg', 36, N'Việt Nam', 10, 1, 1, GETDATE(), GETDATE());
                ELSE UPDATE [SanPhams] SET [TrangThai] = 1, [NoiBat] = 1, [SoLuongTon] = CASE WHEN [SoLuongTon] <= 0 THEN 50 ELSE [SoLuongTon] END, [GiaBan] = CASE WHEN [GiaBan] <= 0 THEN 3990000 ELSE [GiaBan] END, [DanhMucId] = CASE WHEN [DanhMucId] IS NULL THEN @LinhKienId ELSE [DanhMucId] END WHERE [MaSanPham] = N'CPU001';

                IF NOT EXISTS (SELECT 1 FROM [SanPhams] WHERE [MaSanPham] = N'GPU001')
                    INSERT INTO [SanPhams] ([MaSanPham], [TenSanPham], [Slug], [MoTaNgan], [DanhMucId], [ThuongHieu], [GiaNhap], [GiaBan], [GiaKhuyenMai], [SoLuongTon], [DonViTinh], [HinhAnh], [BaoHanh], [XuatXu], [SoLuongDaBan], [NoiBat], [TrangThai], [NgayTao], [NgayCapNhat])
                    VALUES (N'GPU001', N'VGA ASUS RTX 3060 12GB', N'vga-asus-rtx-3060-12gb', N'Card đồ họa NVIDIA RTX 3060 12GB GDDR6', @LinhKienId, N'ASUS', 5000000, 7990000, 7490000, 30, N'Cái', N'/images/gpu-rtx3060.jpg', 36, N'Việt Nam', 5, 1, 1, GETDATE(), GETDATE());
                ELSE UPDATE [SanPhams] SET [TrangThai] = 1, [NoiBat] = 1, [SoLuongTon] = CASE WHEN [SoLuongTon] <= 0 THEN 30 ELSE [SoLuongTon] END, [GiaBan] = CASE WHEN [GiaBan] <= 0 THEN 7990000 ELSE [GiaBan] END WHERE [MaSanPham] = N'GPU001';

                IF NOT EXISTS (SELECT 1 FROM [SanPhams] WHERE [MaSanPham] = N'SSD001')
                    INSERT INTO [SanPhams] ([MaSanPham], [TenSanPham], [Slug], [MoTaNgan], [DanhMucId], [ThuongHieu], [GiaNhap], [GiaBan], [GiaKhuyenMai], [SoLuongTon], [DonViTinh], [HinhAnh], [BaoHanh], [XuatXu], [SoLuongDaBan], [NoiBat], [TrangThai], [NgayTao], [NgayCapNhat])
                    VALUES (N'SSD001', N'SSD Samsung 980 NVMe 500GB', N'ssd-samsung-980-nvme-500gb', N'Ổ cứng NVMe tốc độ cao 500GB', @LinhKienId, N'Samsung', 600000, 990000, 890000, 100, N'Cái', N'/images/ssd-samsung.jpg', 60, N'Việt Nam', 25, 1, 1, GETDATE(), GETDATE());
                ELSE UPDATE [SanPhams] SET [TrangThai] = 1, [NoiBat] = 1, [SoLuongTon] = CASE WHEN [SoLuongTon] <= 0 THEN 100 ELSE [SoLuongTon] END, [GiaBan] = CASE WHEN [GiaBan] <= 0 THEN 990000 ELSE [GiaBan] END WHERE [MaSanPham] = N'SSD001';

                IF NOT EXISTS (SELECT 1 FROM [SanPhams] WHERE [MaSanPham] = N'ACC001')
                    INSERT INTO [SanPhams] ([MaSanPham], [TenSanPham], [Slug], [MoTaNgan], [DanhMucId], [ThuongHieu], [GiaNhap], [GiaBan], [GiaKhuyenMai], [SoLuongTon], [DonViTinh], [HinhAnh], [BaoHanh], [XuatXu], [SoLuongDaBan], [NoiBat], [TrangThai], [NgayTao], [NgayCapNhat])
                    VALUES (N'ACC001', N'Cáp sạc nhanh Type-C to Lightning 20W', N'cap-sac-nhanh-type-c-to-lightning-20w', N'Cáp sạc nhanh chất lượng cao', @PhuKienDienThoaiId, N'VieITStore', 80000, 150000, 129000, 200, N'Cái', N'/images/cable-typec.jpg', 12, N'Việt Nam', 60, 0, 1, GETDATE(), GETDATE());
                ELSE
                    UPDATE [SanPhams]
                    SET [TrangThai] = 1,
                        [SoLuongTon] = CASE WHEN [SoLuongTon] <= 0 THEN 200 ELSE [SoLuongTon] END,
                        [GiaNhap] = CASE WHEN [GiaNhap] < 0 THEN 80000 ELSE [GiaNhap] END,
                        [GiaBan] = CASE WHEN [GiaBan] <= 0 THEN 150000 ELSE [GiaBan] END
                    WHERE [MaSanPham] = N'ACC001';

                IF NOT EXISTS (SELECT 1 FROM [SanPhams] WHERE [MaSanPham] = N'ACC002')
                    INSERT INTO [SanPhams] ([MaSanPham], [TenSanPham], [Slug], [MoTaNgan], [DanhMucId], [ThuongHieu], [GiaNhap], [GiaBan], [GiaKhuyenMai], [SoLuongTon], [DonViTinh], [HinhAnh], [BaoHanh], [XuatXu], [SoLuongDaBan], [NoiBat], [TrangThai], [NgayTao], [NgayCapNhat])
                    VALUES (N'ACC002', N'Chuột Logitech G502 HERO', N'chuot-logitech-g502-hero', N'Chuột gaming Logitech G502 HERO chuyên nghiệp', @PhuKienMayTinhId, N'Logitech', 600000, 899000, NULL, 45, N'Cái', N'/images/mouse-logitech.jpg', 24, N'Việt Nam', 15, 0, 1, GETDATE(), GETDATE());
                ELSE
                    UPDATE [SanPhams]
                    SET [TrangThai] = 1,
                        [SoLuongTon] = CASE WHEN [SoLuongTon] <= 0 THEN 45 ELSE [SoLuongTon] END,
                        [GiaNhap] = CASE WHEN [GiaNhap] < 0 THEN 600000 ELSE [GiaNhap] END,
                        [GiaBan] = CASE WHEN [GiaBan] <= 0 THEN 899000 ELSE [GiaBan] END
                    WHERE [MaSanPham] = N'ACC002';
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Dữ liệu sửa chữa có thể đã được người dùng tham chiếu; không tự động xóa khi rollback.
        }
    }
}

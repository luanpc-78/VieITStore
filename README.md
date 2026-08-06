# VieITStore - Hệ thống quản lý bán hàng Linh kiện điện tử & Phụ kiện

**VieITStore** là ứng dụng web quản lý bán hàng chuyên biệt cho lĩnh vực linh kiện điện tử và phụ kiện công nghệ, được xây dựng trên nền tảng **ASP.NET Core MVC** với ngôn ngữ **C#**. Hệ thống hỗ trợ đầy đủ quy trình từ quản lý kho, bán hàng trực tiếp đến mua sắm trực tuyến, phù hợp với môi trường kinh doanh thực tế tại Việt Nam.

---

## Thành viên nhóm phát triển

| STT | Họ và tên | Vai trò |
|:---:|:---|:---|
| 1 | **Phan Công Luận** | Trưởng nhóm / Backend Developer |
| 2 | **Võ Văn Đức** | Frontend Developer / UI-UX Designer |
| 3 | **Nguyễn Quang Hưng** | Database Developer / Tester |

---

## Mục lục

- [Tính năng chính](#tính-năng-chính)
- [Công nghệ sử dụng](#công-nghệ-sử-dụng)
- [Yêu cầu hệ thống](#yêu-cầu-hệ-thống)
- [Hướng dẫn cài đặt](#hướng-dẫn-cài-đặt)
- [Cấu trúc dự án](#cấu-trúc-dự-án)
- [Phân quyền người dùng](#phân-quyền-người-dùng)
- [Tài khoản demo](#tài-khoản-demo)
- [Hướng dẫn sử dụng](#hướng-dẫn-sử-dụng)
- [Database Schema](#database-schema)
- [API Endpoints](#api-endpoints)
- [Hình ảnh giao diện](#hình-ảnh-giao-diện)
- [Liên hệ](#liên-hệ)

---

## Tính năng chính

###  For Khách hàng (Người mua hàng)
- [x] **Trang chủ** hiển thị sản phẩm nổi bật, sản phẩm mới
- [x] **Danh mục sản phẩm** - Lọc theo danh mục, giá, bán chạy
- [x] **Tìm kiếm sản phẩm** theo tên, mã sản phẩm
- [x] **Chi tiết sản phẩm** - Thông tin đầy đủ, đánh giá
- [x] **Giỏ hàng** - Thêm/xóa/sửa số lượng (hỗ trợ session cho khách vãng lai)
- [x] **Thanh toán đa phương thức**: COD, Chuyển khoản, Ví điện tử
- [x] **Lưu địa chỉ giao hàng** cho lần mua sau
- [x] **Theo dõi đơn hàng** - Xem trạng thái đơn hàng real-time
- [x] **Hủy đơn hàng** (khi đơn chưa giao)
- [x] **Đánh giá sản phẩm** sau khi nhận hàng
- [x] **Đăng ký / Đăng nhập / Đăng xuất**

###  For Nhân viên
- [x] **Dashboard** - Tổng quan doanh thu, đơn hàng, tồn kho
- [x] **Quản lý đơn hàng** - Xác nhận, cập nhật trạng thái giao hàng
- [x] **Quản lý sản phẩm** - Thêm, sửa, xóa sản phẩm
- [x] **Nhập hàng từ NCC** - Tạo phiếu nhập, cập nhật tồn kho tự động
- [x] **Cảnh báo tồn kho thấp** - Sản phẩm dưới ngưỡng an toàn

###  For Admin
- [x] Toàn bộ quyền của Nhân viên
- [x] **Quản lý người dùng** - Phân quyền, khóa/mở tài khoản
- [x] **Quản lý nhà cung cấp**
- [x] **Báo cáo doanh thu** theo ngày/tháng/năm
- [x] **Thống kê biểu đồ** (có thể mở rộng)

---

## Công nghệ sử dụng

| Lớp | Công nghệ | Phiên bản |
|:---|:---|:---:|
| **Backend** | ASP.NET Core MVC | 8.0 |
| **Ngôn ngữ** | C# | 12.0 |
| **Database** | Microsoft SQL Server | 2022 / LocalDB |
| **ORM** | Entity Framework Core | 8.0 |
| **Frontend** | HTML5, CSS3, JavaScript | - |
| **UI Framework** | Bootstrap | 5.3 |
| **Icons** | Font Awesome | 6.4 |
| **Authentication** | Session-based + BCrypt | - |
| **IDE** | Visual Studio 2022 | 17.x |

---

## Yêu cầu hệ thống

### Phần mềm
- **Hệ điều hành**: Windows 10/11 (64-bit)
- **Visual Studio**: 2022 Community/Professional/Enterprise
- **.NET SDK**: 8.0 trở lên
- **SQL Server**: 2022 Express / LocalDB / Developer Edition
- **Trình duyệt**: Chrome, Edge, Firefox (phiên bản mới nhất)

### Phần cứng (tối thiểu)
- **RAM**: 4 GB
- **Ổ cứng**: 2 GB dung lượng trống
- **CPU**: Intel Core i3 hoặc tương đương

---

## Hướng dẫn cài đặt

### Bước 1: Clone hoặc tải source code
```bash
git clone https://github.com/your-repo/VieITStore.git
```
Hoặc tải file ZIP và giải nén.

### Bước 2: Mở project trong Visual Studio 2022
1. Mở Visual Studio 2022
2. Chọn **File** → **Open** → **Project/Solution**
3. Chọn file `VieITStore.sln`

### Bước 3: Cài đặt NuGet Packages
Mở **Package Manager Console** (Tools → NuGet Package Manager → Package Manager Console), chạy:
```powershell
Install-Package BCrypt.Net-Next
Install-Package Microsoft.EntityFrameworkCore.SqlServer -Version 8.0.0
Install-Package Microsoft.EntityFrameworkCore.Tools -Version 8.0.0
```

### Bước 4: Cấu hình Database

#### Cách A: Dùng Migration (Entity Framework)
```powershell
Add-Migration InitialCreate
Update-Database
```

#### Cách B: Dùng file SQL Script
1. Mở **SQL Server Management Studio (SSMS)**
2. File → Open → File → Chọn `VieITStore_Database.sql`
3. Nhấn **Execute** (F5)

### Bước 5: Cấu hình Connection String
Kiểm tra file `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=VieITStoreDB;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```
> Sửa `Server` phù hợp với SQL Server của bạn (ví dụ: `Server=localhost\SQLEXPRESS`)

### Bước 6: Chạy ứng dụng
Nhấn **F5** hoặc **Ctrl+F5** để chạy project.

Truy cập: `https://localhost:xxxx` hoặc `http://localhost:xxxx`

---

## Cấu trúc dự án

```
VieITStore/
│
├── Areas/
│   └── Admin/
│       ├── Controllers/          # Controllers cho khu vực Admin
│       │   ├── DashboardController.cs
│       │   ├── DonHangAdminController.cs
│       │   ├── SanPhamAdminController.cs
│       │   └── NhapHangController.cs
│       └── Views/                # Views cho khu vực Admin
│           ├── Dashboard/
│           ├── DonHangAdmin/
│           ├── SanPhamAdmin/
│           ├── NhapHang/
│           └── Shared/
│               └── _LayoutAdmin.cshtml
│
├── Controllers/                  # Controllers cho khu vực Khách hàng
│   ├── HomeController.cs
│   ├── SanPhamController.cs
│   ├── GioHangController.cs
│   ├── DonHangController.cs
│   ├── ThanhToanController.cs
│   └── TaiKhoanController.cs
│
├── Filters/                      # Custom Authorization Filters
│   └── AuthorizeRoleAttribute.cs
│
├── Models/
│   ├── Entities/                 # Các class Entity (Table)
│   │   ├── DanhMuc.cs
│   │   ├── SanPham.cs
│   │   ├── NguoiDung.cs
│   │   ├── KhachHang.cs
│   │   ├── DiaChiGiaoHang.cs
│   │   ├── GioHang.cs
│   │   ├── DonHang.cs
│   │   ├── ChiTietDonHang.cs
│   │   ├── NhaCungCap.cs
│   │   ├── PhieuNhap.cs
│   │   ├── ChiTietPhieuNhap.cs
│   │   └── DanhGia.cs
│   ├── ViewModels/               # Các class ViewModel
│   │   ├── LoginVM.cs
│   │   ├── RegisterVM.cs
│   │   ├── GioHangVM.cs
│   │   └── ThanhToanVM.cs
│   └── ApplicationDbContext.cs   # DbContext (EF Core)
│
├── Views/                        # Views cho khu vực Khách hàng
│   ├── Shared/
│   │   ├── _Layout.cshtml
│   │   └── _ValidationScriptsPartial.cshtml
│   ├── Home/
│   ├── SanPham/
│   ├── GioHang/
│   ├── DonHang/
│   ├── ThanhToan/
│   └── TaiKhoan/
│
├── wwwroot/                      # Static files (CSS, JS, Images)
│   ├── css/
│   ├── js/
│   └── images/
│
├── VieITStore.csproj             # Project file
├── appsettings.json              # Configuration
├── Program.cs                    # Entry point
└── VieITStore_Database.sql       # SQL Script tạo Database
```

---

## Phân quyền người dùng

| Vai trò | Mô tả | Quyền hạn |
|:---|:---|:---|
| **Admin** | Quản trị viên hệ thống | Toàn quyền quản lý |
| **NhanVien** | Nhân viên bán hàng/kho | Quản lý đơn hàng, sản phẩm, nhập hàng |
| **KhachHang** | Khách hàng mua online | Xem sản phẩm, mua hàng, theo dõi đơn |

---

## Tài khoản demo

| Vai trò | Tên đăng nhập | Mật khẩu |
|:---|:---|:---|
| **Admin** | `admin` | `admin123` |
| **Nhân viên** | `nhanvien` | `nhanvien123` |
| **Khách hàng 1** | `khachhang1` | `khachhang123` |
| **Khách hàng 2** | `khachhang2` | `khachhang123` |

> **Lưu ý**: Trong môi trường production, vui lòng thay đổi mật khẩu mặc định và sử dụng BCrypt hash.

---

## Hướng dẫn sử dụng

###  Khách hàng mua hàng
1. Truy cập trang chủ, duyệt sản phẩm hoặc tìm kiếm
2. Click **"Thêm vào giỏ"** để chọn sản phẩm
3. Vào **Giỏ hàng** để kiểm tra và điều chỉnh số lượng
4. Click **"Thanh toán"** và điền thông tin giao hàng
5. Chọn phương thức thanh toán (COD / Chuyển khoản / Ví điện tử)
6. Click **"Đặt hàng"** để hoàn tất

###  Nhân viên xử lý đơn hàng
1. Đăng nhập với tài khoản Nhân viên
2. Vào **Quản trị** → **Đơn hàng**
3. Xem đơn hàng mới, click **"Cập nhật"** để đổi trạng thái
4. Các trạng thái: Chờ xác nhận → Đã xác nhận → Đang giao → Đã giao → Hoàn thành

###  Nhân viên nhập hàng
1. Vào **Quản trị** → **Nhập hàng** → **Nhập hàng mới**
2. Chọn nhà cung cấp, thêm sản phẩm nhập
3. Nhập đơn giá và số lượng
4. Lưu phiếu nhập → Tồn kho tự động cập nhật

---

## Database Schema

### Sơ đồ quan hệ (ER Diagram)

```
┌─────────────┐     ┌─────────────┐     ┌─────────────┐
│  DanhMucs   │◄────┤  SanPhams   │◄────┤  DanhGias   │
└─────────────┘     └──────┬──────┘     └─────────────┘
                           │
           ┌───────────────┼───────────────┐
           │               │               │
           ▼               ▼               ▼
    ┌─────────────┐ ┌─────────────┐ ┌─────────────┐
    │ChiTietDonHang│ │ChiTietPhieuNhap│  │   GioHangs   │
    └──────┬──────┘ └──────┬──────┘ └──────┬──────┘
           │               │               │
           ▼               ▼               ▼
    ┌─────────────┐ ┌─────────────┐ ┌─────────────┐
    │  DonHangs   │ │ PhieuNhaps  │ │  NguoiDungs │
    └──────┬──────┘ └──────┬──────┘ └──────┬──────┘
           │               │               │
           │         ┌─────┘               │
           │         ▼                     │
           │    ┌─────────────┐            │
           │    │ NhaCungCaps │            │
           │    └─────────────┘            │
           │                               │
           └───────────────┬───────────────┘
                           ▼
                    ┌─────────────┐
                    │ KhachHangs  │
                    └──────┬──────┘
                           ▼
                    ┌─────────────┐
                    │DiaChiGiaoHang│
                    └─────────────┘
```

### Các bảng chính

| Bảng | Mô tả | Số dòng mẫu |
|:---|:---|:---:|
| `NguoiDungs` | Tài khoản hệ thống | 4 |
| `DanhMucs` | Danh mục sản phẩm | 4 |
| `SanPhams` | Thông tin sản phẩm | 10 |
| `KhachHangs` | Profile khách hàng | 2 |
| `DiaChiGiaoHangs` | Địa chỉ giao hàng | 3 |
| `GioHangs` | Giỏ hàng | - |
| `DonHangs` | Đơn hàng | 4 |
| `ChiTietDonHangs` | Chi tiết đơn hàng | 7 |
| `NhaCungCaps` | Nhà cung cấp | 3 |
| `PhieuNhaps` | Phiếu nhập hàng | 2 |
| `ChiTietPhieuNhaps` | Chi tiết phiếu nhập | 8 |
| `DanhGias` | Đánh giá sản phẩm | 4 |

---

## API Endpoints

### Giỏ hàng (AJAX)
| Method | Endpoint | Mô tả |
|:---:|:---|:---|
| POST | `/GioHang/ThemVaoGio` | Thêm sản phẩm vào giỏ |
| POST | `/GioHang/CapNhatSoLuong` | Cập nhật số lượng |
| POST | `/GioHang/XoaSanPham` | Xóa sản phẩm khỏi giỏ |
| GET | `/GioHang/GetCartCount` | Lấy số lượng giỏ hàng |

### Admin (AJAX)
| Method | Endpoint | Mô tả |
|:---:|:---|:---|
| POST | `/Admin/DonHangAdmin/CapNhatTrangThai` | Cập nhật trạng thái đơn hàng |

---

## Hình ảnh giao diện

### Trang chủ
- Banner quảng cáo sản phẩm nổi bật
- Danh sách sản phẩm theo danh mục
- Thanh tìm kiếm và giỏ hàng

### Trang sản phẩm
- Lưới sản phẩm với hình ảnh, giá, trạng thái tồn kho
- Bộ lọc theo danh mục, giá, bán chạy
- Nút thêm vào giỏ hàng

### Trang thanh toán
- Form thông tin giao hàng
- Chọn địa chỉ đã lưu
- Chọn phương thức thanh toán
- Tóm tắt đơn hàng

### Trang quản trị (Admin)
- Dashboard với thống kê tổng quan
- Bảng quản lý đơn hàng với trạng thái màu sắc
- Form nhập hàng động (thêm/xóa dòng)

---

## Tính năng đặc biệt

###  Phù hợp thực tế Việt Nam
- Đơn vị tiền tệ **VNĐ** (format: `1.234.567 đ`)
- Ngày tháng định dạng **dd/MM/yyyy**
- Địa chỉ: Tỉnh/Thành phố → Quận/Huyện → Phường/Xã
- Phí vận chuyển mặc định **30.000đ**
- Hỗ trợ thanh toán: **COD, Chuyển khoản, Momo, ZaloPay, VNPay**

###  Cảnh báo thông minh
- Tự động cảnh báo sản phẩm sắp hết hàng (≤ 10 sản phẩm)
- Highlight đơn hàng theo trạng thái màu sắc
- Toast notification khi thao tác thành công

###  Bảo mật
- Mã hóa mật khẩu bằng **BCrypt**
- Phân quyền **Role-based** (Admin/NhanVien/KhachHang)
- Session timeout sau 2 giờ
- Anti-forgery token cho form

---

## Các lệnh hữu ích

### Migration
```powershell
# Tạo migration mới
Add-Migration TenMigration

# Cập nhật database
Update-Database

# Xóa migration cuối cùng
Remove-Migration

# Tạo script SQL từ migration
Script-Migration
```

### Build & Run
```bash
# Build project
dotnet build

# Run project
dotnet run

# Publish project
dotnet publish -c Release
```

---

## Lộ trình phát triển (Roadmap)

### Phase 1 (Hiện tại)
- [x] Quản lý sản phẩm, đơn hàng, nhập hàng
- [x] Giỏ hàng và thanh toán cơ bản
- [x] Phân quyền 3 cấp

### Phase 2 (Dự kiến)
- [ ] Tích hợp thanh toán VNPay/Momo API thật
- [ ] Gửi email/SMS thông báo đơn hàng
- [ ] Xuất hóa đơn PDF
- [ ] Quản lý khuyến mãi, voucher

### Phase 3 (Dự kiến)
- [ ] Ứng dụng di động (MAUI/Flutter)
- [ ] Chatbot hỗ trợ khách hàng
- [ ] Tích hợp kho vận Giao Hàng Nhanh
- [ ] Phân tích dữ liệu bán hàng (Power BI)

---

## Lỗi thường gặp & Cách khắc phục

| Lỗi | Nguyên nhân | Cách khắc phục |
|:---|:---|:---|
| `Filters does not exist` | Thiếu folder Filters | Tạo folder `Filters/` và file `AuthorizeRoleAttribute.cs` |
| `Database connection failed` | Sai connection string | Kiểm tra `appsettings.json`, đảm bảo SQL Server đang chạy |
| `BCrypt verify failed` | Hash không khớp | Sửa code để hỗ trợ plain text password trong dev mode |
| `The tag helper 'option' must not have C#` | Razor syntax sai | Dùng `selected="@(condition)"` thay vì `selected="@condition"` |
| `CSS margin-left invalid` | Biến CSS không hợp lệ | Dùng `calc()` hoặc giá trị cố định |

---

## Đóng góp

Mọi đóng góp đều được hoan nghênh! Vui lòng:
1. Fork repository
2. Tạo branch mới (`git checkout -b feature/AmazingFeature`)
3. Commit thay đổi (`git commit -m 'Add some AmazingFeature'`)
4. Push lên branch (`git push origin feature/AmazingFeature`)
5. Mở Pull Request

---

## Giấy phép

Dự án này được phát triển cho mục đích học tập và nghiên cứu.

---

## Liên hệ

| Thành viên | Email |
|:---|:---|
| Phan Công Luận | phancongluan@example.com |
| Võ Văn Đức | vovanduc@example.com |
| Nguyễn Quang Hưng | nguyenquanghung@example.com |

**Dự án**: VieITStore  
**Trường**: [Tên trường]  
**Môn học**: Lập trình ứng dụng Web  
**Năm học**: 2024 - 2025

---

<p align="center">
  <strong>Made with  by VieIT Team</strong>
</p>

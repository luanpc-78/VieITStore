# VieITStore

## Cập nhật cơ sở dữ liệu

Mặc định ứng dụng không tự áp migration khi khởi động ngoài môi trường Development. Khi triển khai, chạy:

```powershell
dotnet ef database update --project VieITStore/VieITStore.csproj --startup-project VieITStore/VieITStore.csproj
```

Chỉ bật tự động migration khi chủ động đặt cấu hình `Database:ApplyMigrationsOnStartup` thành `true`. Nếu migration thất bại, ứng dụng sẽ dừng khởi động thay vì tiếp tục với schema không đồng bộ.

# Hệ Thống Quản Lý Thuê Xe Mô Tô

## Giới thiệu
Đây là hệ thống quản lý cho thuê xe mô tô được xây dựng bằng ASP.NET Core MVC với SQL Server.

## Công nghệ sử dụng
- ASP.NET Core MVC (.NET 10)
- Entity Framework Core
- SQL Server
- Bootstrap 5
- Font Awesome 6
- BCrypt.Net (mã hóa mật khẩu)

## Cấu trúc dự án
```
QuanLyThueXe/
├── Areas/
│   └── Admin/          # Khu vực quản trị
│       ├── Controllers/
│       └── Views/
├── Controllers/        # Controllers cho khách hàng
├── Data/              # DbContext
├── Models/            # Models và ViewModels
├── Views/             # Views cho khách hàng
└── wwwroot/           # Static files
```

## Hướng dẫn cài đặt và chạy

### Bước 1: Cài đặt SQL Server
1. Tải và cài đặt SQL Server (Express hoặc Developer Edition)
2. Tải và cài đặt SQL Server Management Studio (SSMS)

### Bước 2: Tạo Database
1. Mở SQL Server Management Studio
2. Kết nối đến SQL Server (localhost hoặc .\SQLEXPRESS)
3. Mở file `database.sql` trong thư mục gốc
4. Chạy toàn bộ script để tạo database và dữ liệu mẫu

### Bước 3: Cấu hình Connection String
1. Mở file `appsettings.json`
2. Cập nhật connection string phù hợp với SQL Server của bạn:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=QuanLyThueXeDB;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
}
```

**Lưu ý:** 
- Nếu dùng SQL Server Express: `Server=.\\SQLEXPRESS`
- Nếu dùng SQL Authentication: `Server=localhost;Database=QuanLyThueXeDB;User Id=sa;Password=yourpassword;TrustServerCertificate=True`

### Bước 4: Chạy ứng dụng
```bash
cd QuanLyThueXe
dotnet restore
dotnet run
```

Hoặc mở bằng Visual Studio và nhấn F5.

### Bước 5: Truy cập ứng dụng
- Trang chủ: https://localhost:5001 hoặc http://localhost:5000
- Trang Admin: https://localhost:5001/Admin/Dashboard

## Tài khoản mặc định

### Admin
- Email: admin@thuexe.vn
- Mật khẩu: Admin@123456

### Khách hàng
- Email: levan.an@gmail.com
- Mật khẩu: KhachHang@123456

## Chức năng chính

### Dành cho Admin
1. **Dashboard**: Thống kê tổng quan
2. **Quản lý xe**: Thêm, sửa, xóa thông tin xe
3. **Quản lý khách hàng**: Xem và cập nhật thông tin khách hàng
4. **Quản lý hợp đồng**: Xác nhận, hủy, trả xe
5. **Quản lý bảo dưỡng**: Theo dõi lịch bảo dưỡng xe
6. **Quản lý khuyến mãi**: Tạo và quản lý mã khuyến mãi
7. **Quản lý đánh giá**: Duyệt và ẩn/hiện đánh giá
8. **Báo cáo**: Xem báo cáo doanh thu, thống kê

### Dành cho Khách hàng
1. **Xem danh sách xe**: Tìm kiếm, lọc xe theo tiêu chí
2. **Xem chi tiết xe**: Thông tin chi tiết, hình ảnh, đánh giá
3. **Đặt xe**: Tạo hợp đồng thuê xe
4. **Quản lý hợp đồng**: Xem lịch sử thuê xe
5. **Đánh giá xe**: Đánh giá sau khi trả xe
6. **Hồ sơ cá nhân**: Cập nhật thông tin cá nhân

## Cấu trúc Database

### Bảng chính
- **NGUOI_DUNG**: Thông tin người dùng
- **VAI_TRO**: Vai trò (Admin, KhachHang)
- **QUYEN**: Quyền hạn
- **KHACH_HANG**: Thông tin khách hàng
- **XE**: Thông tin xe
- **HOP_DONG**: Hợp đồng thuê xe
- **THANH_TOAN**: Thanh toán
- **DANH_GIA**: Đánh giá xe
- **BAO_DUONG**: Bảo dưỡng xe
- **KHUYEN_MAI**: Khuyến mãi
- **THONG_BAO**: Thông báo

## Lưu ý
- Đảm bảo SQL Server đang chạy trước khi khởi động ứng dụng
- Kiểm tra firewall nếu không kết nối được database
- Thư mục `wwwroot/images/xe` cần có quyền ghi để upload hình ảnh

## Liên hệ
- Nhóm 9
- Email: admin@thuexe.vn

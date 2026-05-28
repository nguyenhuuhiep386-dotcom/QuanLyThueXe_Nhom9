# HƯỚNG DẪN TÙY CHỈNH GIAO DIỆN

## Tổng quan

Dự án đã được thiết kế với giao diện hiện đại sử dụng:
- Bootstrap 5
- Font Awesome 6
- CSS tùy chỉnh (custom.css)

## Cấu trúc CSS

File CSS chính: `wwwroot/css/custom.css`

### Biến CSS (CSS Variables)
```css
:root {
    --primary-color: #2563eb;      /* Màu chủ đạo */
    --secondary-color: #64748b;    /* Màu phụ */
    --success-color: #10b981;      /* Màu thành công */
    --danger-color: #ef4444;       /* Màu nguy hiểm */
    --warning-color: #f59e0b;      /* Màu cảnh báo */
    --info-color: #06b6d4;         /* Màu thông tin */
}
```

## Tùy chỉnh theo hình ảnh giao diện

### 1. Đối với Admin Dashboard (1_Dashboard.png)

**Cách tùy chỉnh:**
1. Mở file: `Areas/Admin/Views/Dashboard/Index.cshtml`
2. Chỉnh sửa các stats-card:
```html
<div class="stats-card">
    <div class="stats-icon">
        <i class="fas fa-motorcycle"></i>
    </div>
    <div class="stats-number">100</div>
    <div class="stats-label">Tổng số xe</div>
</div>
```

3. Thay đổi màu sắc trong `custom.css`:
```css
.stats-card {
    border-left-color: #your-color;
}
```

### 2. Đối với Quản lý Xe (2_QuanLyXe.png)

**Cách tùy chỉnh:**
1. Mở file: `Areas/Admin/Views/Xe/Index.cshtml`
2. Chỉnh sửa bảng và các nút:
```html
<table class="table table-hover">
    <!-- Thêm/sửa cột theo hình ảnh -->
</table>
```

### 3. Đối với Trang chủ Khách hàng (2_Trang chủ - Models.png)

**Cách tùy chỉnh:**
1. Mở file: `Views/Home/Index.cshtml`
2. Chỉnh sửa hero section:
```html
<div class="hero-section">
    <h1>Tiêu đề của bạn</h1>
    <p>Mô tả của bạn</p>
</div>
```

3. Thay đổi gradient trong `custom.css`:
```css
.hero-section {
    background: linear-gradient(135deg, #màu1 0%, #màu2 100%);
}
```

### 4. Đối với Chi tiết xe (5_Chi tiết xe.png)

**Cách tùy chỉnh:**
1. Mở file: `Views/Xe/ChiTiet.cshtml`
2. Chỉnh sửa layout hình ảnh và thông tin

## Hướng dẫn chi tiết từng bước

### Bước 1: Xem hình ảnh giao diện mẫu
Mở các file PNG trong thư mục `Giao diện/`

### Bước 2: Xác định các thành phần cần thay đổi
- Màu sắc
- Bố cục (layout)
- Font chữ
- Kích thước
- Icon

### Bước 3: Chỉnh sửa CSS
Mở file `wwwroot/css/custom.css` và tìm class tương ứng:

**Ví dụ thay đổi màu primary:**
```css
:root {
    --primary-color: #FF6B6B;  /* Đổi từ xanh sang đỏ */
}
```

**Ví dụ thay đổi border-radius:**
```css
:root {
    --border-radius: 20px;  /* Đổi từ 12px sang 20px */
}
```

### Bước 4: Chỉnh sửa HTML
Mở file View tương ứng và chỉnh sửa cấu trúc HTML

**Ví dụ thêm một card:**
```html
<div class="col-md-4">
    <div class="card">
        <div class="card-body">
            <h5>Tiêu đề</h5>
            <p>Nội dung</p>
        </div>
    </div>
</div>
```

### Bước 5: Test và điều chỉnh
1. Chạy ứng dụng: `dotnet run`
2. Mở trình duyệt và xem kết quả
3. Sử dụng Developer Tools (F12) để inspect và điều chỉnh

## Các class CSS quan trọng

### Cards
```css
.card                 /* Card cơ bản */
.card-header         /* Header của card */
.card-body           /* Body của card */
.xe-card             /* Card hiển thị xe */
.stats-card          /* Card thống kê */
```

### Buttons
```css
.btn                 /* Button cơ bản */
.btn-primary         /* Button màu primary */
.btn-success         /* Button màu success */
.btn-lg              /* Button lớn */
```

### Tables
```css
.table               /* Table cơ bản */
.table-hover         /* Table có hover effect */
.table thead th      /* Header của table */
```

### Forms
```css
.form-control        /* Input field */
.form-select         /* Select dropdown */
.form-label          /* Label của form */
```

## Tùy chỉnh theo từng màn hình

### Admin - Dashboard
File: `Areas/Admin/Views/Dashboard/Index.cshtml`
- Thống kê: Sử dụng `.stats-card`
- Biểu đồ: Có thể thêm Chart.js
- Bảng: Sử dụng `.table`

### Admin - Quản lý Xe
File: `Areas/Admin/Views/Xe/Index.cshtml`
- Bảng danh sách: `.table`
- Nút thao tác: `.btn-group`
- Filter: `.filter-section`

### Khách hàng - Trang chủ
File: `Views/Home/Index.cshtml`
- Hero: `.hero-section`
- Danh sách xe: `.xe-card`
- Khuyến mãi: `.card`

### Khách hàng - Đặt xe
File: `Views/HopDong/DatXe.cshtml`
- Form: `.form-control`
- Tính tiền: `.card.bg-light`
- Upload: `input[type="file"]`

## Tips và Tricks

### 1. Thay đổi màu toàn bộ website
Chỉ cần thay đổi biến CSS trong `:root`

### 2. Thêm animation
```css
.my-element {
    transition: all 0.3s ease;
}

.my-element:hover {
    transform: translateY(-5px);
}
```

### 3. Responsive design
```css
@media (max-width: 768px) {
    .my-element {
        /* CSS cho mobile */
    }
}
```

### 4. Thêm icon
Sử dụng Font Awesome:
```html
<i class="fas fa-motorcycle"></i>
<i class="fas fa-user"></i>
<i class="fas fa-calendar"></i>
```

### 5. Gradient background
```css
background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
```

## Công cụ hỗ trợ

### 1. Chrome DevTools (F12)
- Inspect element
- Chỉnh sửa CSS trực tiếp
- Xem responsive

### 2. Color Picker
- https://htmlcolorcodes.com/
- https://coolors.co/

### 3. Gradient Generator
- https://cssgradient.io/

### 4. Icon Library
- https://fontawesome.com/icons

## Checklist tùy chỉnh

- [ ] Thay đổi màu sắc chủ đạo
- [ ] Điều chỉnh font chữ
- [ ] Chỉnh sửa logo
- [ ] Tùy chỉnh navbar
- [ ] Điều chỉnh footer
- [ ] Chỉnh sửa cards
- [ ] Tùy chỉnh buttons
- [ ] Điều chỉnh forms
- [ ] Chỉnh sửa tables
- [ ] Test responsive

## Liên hệ hỗ trợ

Nếu cần hỗ trợ chi tiết hơn về tùy chỉnh giao diện:
1. Mô tả chi tiết phần cần thay đổi
2. Gửi kèm hình ảnh mẫu
3. Chỉ rõ file cần chỉnh sửa

---
**Chúc bạn tùy chỉnh giao diện thành công!** 🎨

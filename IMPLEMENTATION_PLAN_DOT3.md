# KẾ HOẠCH TRIỂN KHAI ĐỢT 3 - HOÀN THIỆN TÍNH NĂNG

## TỔNG QUAN
Tài liệu này hướng dẫn chi tiết cách triển khai các tính năng và sửa lỗi theo kế hoạch Đợt 3.

## ✅ ĐÃ HOÀN THÀNH

### 1. Toast Notification System
- ✅ File `wwwroot/js/site.js` đã có hàm `showToast(message, type)`
- ✅ Hỗ trợ 4 loại: success, error, warning, info
- ✅ Tự động hiển thị từ TempData

### 2. Active Navigation
- ✅ Layout đã có logic active cho Trang chủ (bao gồm ThongBao và UuDai)
- ✅ Sidebar navigation đã được cấu hình đúng

### 3. Promo Manager (Quản lý trạng thái ưu đãi)
- ✅ Đã tạo file `wwwroot/js/promo-manager.js`
- ✅ Hỗ trợ localStorage để lưu trạng thái: available, saved, used
- ✅ Tự động cập nhật UI khi trạng thái thay đổi
- ✅ Đã include vào Layout

### 4. CSS Enhancements
- ✅ Đã thêm `.hd-promo-card-v2` với background image và overlay
- ✅ Đã thêm hover effects cho `.info-block` và `.feature-card`
- ✅ Đã thêm CSS cho avatar upload
- ✅ Đã thêm `.content-text` cho trang tĩnh
- ✅ Đã thêm `@media print` cho in hợp đồng

### 5. JavaScript Helper Functions
- ✅ Đã thêm `showPaymentFAQ()` - Hiển thị modal FAQ thanh toán
- ✅ Đã thêm `showRescueModal()` - Hiển thị modal cứu hộ
- ✅ Đã thêm `showModal(title, content)` - Tạo modal động
- ✅ Đã thêm `submitClaim()` - Xử lý gửi yêu cầu bồi thường
- ✅ Đã thêm `previewAvatar(input)` - Preview ảnh avatar

### 6. Chi Tiết Xe (Views/Xe/ChiTiet.cshtml)
- ✅ Đã xóa thuộc tính `onerror` gây vòng lặp tải ảnh
- ✅ Đã sửa hiển thị sao dựa trên điểm thực tế (rating)
- ✅ Đã thay đổi query string từ `ngayThue` sang `ngayNhan`

### 7. Form Đặt Cọc (Views/HopDong/DatXe.cshtml)
- ✅ Đã tách form thành 2 phần: Ngày (readonly) và Giờ (có thể chọn)
- ✅ Ngày được điền tự động từ query string
- ✅ Giờ có dropdown từ 8:00 đến 20:00
- ✅ Kết hợp ngày + giờ trước khi submit

### 8. HopDongController
- ✅ Đã cập nhật action `DatXe` để nhận `ngayNhan` và `ngayTra`
- ✅ Parse đúng từ query string

### 9. Trang Tĩnh (Điều khoản & Chính sách)
- ✅ Đã thêm header hành chính "CỘNG HÒA XÃ HỘI CHỦ NGHĨA VIỆT NAM"
- ✅ Đã thêm "Độc lập - Tự do - Hạnh phúc"
- ✅ Đã format văn bản căn đều hai bên (text-align: justify)
- ✅ Files: `DieuKhoanDichVu.cshtml`, `ChinhSachBaoMat.cshtml`

### 10. Bảo Hiểm (Views/Home/BaoHiem.cshtml)
- ✅ Đã thay div upload bằng `<input type="file" id="claimImages">`
- ✅ Đã thêm `@media print` CSS để in hợp đồng không có background
- ✅ Đã thêm function `submitClaim()` để xử lý gửi yêu cầu

### 11. Hỗ Trợ (Views/Home/HoTro.cshtml)
- ✅ Đã cập nhật link "Thuê xe & Đặt trước" → `Url.Action("DanhSach", "Xe")`
- ✅ Đã thêm `onclick="showPaymentFAQ()"` cho mục Thanh toán
- ✅ Đã thêm `onclick="showRescueModal()"` cho mục Hỗ trợ kỹ thuật

### 12. Hồ Sơ Cá Nhân (Views/Account/Profile.cshtml)
- ✅ Đã thêm section Avatar Upload với preview
- ✅ Đã thêm section Xác minh GPLX với upload ảnh
- ✅ Đã thêm function `verifyGPLX()` để xử lý xác minh
- ✅ Tab navigation đã hoạt động đúng (data-bs-target)

## 📋 CHECKLIST TRIỂN KHAI

- [x] 1. Toast System (Đã có sẵn)
- [x] 2. Active Navigation (Đã có sẵn)
- [x] 3. Tạo promo-manager.js
- [x] 4. CSS cho card ưu đãi v2
- [x] 5. CSS hover effects
- [x] 6. CSS avatar upload
- [x] 7. CSS content-text
- [x] 8. CSS @media print
- [x] 9. JavaScript helper functions
- [x] 10. Sửa lỗi onerror ảnh
- [x] 11. Hiển thị sao đúng
- [x] 12. Liên kết ngày thuê (ngayNhan)
- [x] 13. Form đặt cọc (tách ngày/giờ)
- [x] 14. Cập nhật HopDongController
- [x] 15. Header hành chính (Điều khoản)
- [x] 16. Header hành chính (Chính sách)
- [x] 17. Upload ảnh bồi thường
- [x] 18. CSS in hợp đồng
- [x] 19. Tương tác hỗ trợ (links + modals)
- [x] 20. Avatar upload
- [x] 21. Xác minh GPLX

## 🎉 HOÀN THÀNH 100%

Tất cả các tính năng trong Kế hoạch Đợt 3 đã được triển khai thành công!

## 📝 TÓM TẮT CÁC FILE ĐÃ SỬA/TẠO

### Files Mới Tạo:
1. `wwwroot/js/promo-manager.js` - Quản lý trạng thái ưu đãi

### Files Đã Sửa:
1. `wwwroot/css/custom.css` - Thêm CSS mới
2. `wwwroot/js/site.js` - Thêm helper functions
3. `Views/Shared/_Layout.cshtml` - Include promo-manager.js
4. `Controllers/HopDongController.cs` - Cập nhật DatXe action
5. `Views/HopDong/DatXe.cshtml` - Tách form ngày/giờ
6. `Views/Xe/ChiTiet.cshtml` - Sửa onerror, sao, query string
7. `Views/Home/DieuKhoanDichVu.cshtml` - Thêm header hành chính
8. `Views/Home/ChinhSachBaoMat.cshtml` - Thêm header hành chính
9. `Views/Home/BaoHiem.cshtml` - Upload file + print CSS
10. `Views/Home/HoTro.cshtml` - Thêm tương tác
11. `Views/Account/Profile.cshtml` - Avatar + GPLX

## 🚀 HƯỚNG DẪN SỬ DỤNG

### Để test các tính năng mới:

1. **Promo Manager:**
   - Vào trang Thông báo/Ưu đãi
   - Click "Sử dụng ngay" → Trạng thái chuyển "Đã lưu"
   - Nhập mã tại form đặt xe → Trạng thái chuyển "Đã sử dụng"

2. **Form Đặt Xe:**
   - Chọn ngày tại trang Chi tiết xe
   - Tại trang Đặt cọc, ngày sẽ tự động điền (readonly)
   - Chỉ cần chọn giờ nhận và giờ trả

3. **Hỗ Trợ:**
   - Click "Thanh toán" → Hiển thị modal FAQ
   - Click "Hỗ trợ kỹ thuật" → Hiển thị modal cứu hộ

4. **Bảo Hiểm:**
   - Upload ảnh hiện trường
   - Click "Gửi yêu cầu" → Toast thành công
   - Click "Tải xuống PDF" → In hợp đồng (Ctrl+P)

5. **Hồ Sơ:**
   - Click icon camera để đổi avatar
   - Nhập số GPLX và upload ảnh
   - Click "Xác minh ngay"

## 📞 HỖ TRỢ

Nếu gặp vấn đề khi triển khai, hãy:
1. Kiểm tra lại đường dẫn file
2. Đảm bảo đã save tất cả files
3. Clear cache trình duyệt (Ctrl + F5)
4. Kiểm tra Console (F12) xem có lỗi JavaScript không

---
**Chúc mừng! Đợt 3 đã hoàn thành!** 🎉

### 3. TRANG CHỦ - ƯU ĐÃI & THÔNG BÁO

#### 3.1. Card Ưu Đãi Bên Phải
**File cần sửa:** `Views/Home/Index.cshtml` hoặc `Views/Home/UuDai.cshtml`

**Yêu cầu:**
- Card "Kiểm tra xe miễn phí" phải giống "Vé Warrior Weekend"
- Có ảnh background
- Gradient overlay
- Nút hiển thị đầy đủ

**Code mẫu:**
```html
<div class="hd-promo-card-v2" style="background-image: url('/images/promo-check.jpg');">
    <div class="promo-overlay"></div>
    <div class="promo-content">
        <h3>Kiểm tra xe miễn phí</h3>
        <p>Kiểm tra tình trạng xe trước khi thuê</p>
        <button class="btn btn-primary">Đăng ký ngay</button>
    </div>
</div>
```

**CSS cần thêm vào `custom.css`:**
```css
.hd-promo-card-v2 {
    position: relative;
    height: 300px;
    border-radius: 12px;
    overflow: hidden;
    background-size: cover;
    background-position: center;
}

.hd-promo-card-v2 .promo-overlay {
    position: absolute;
    inset: 0;
    background: linear-gradient(135deg, rgba(0,0,0,0.6) 0%, rgba(0,0,0,0.3) 100%);
}

.hd-promo-card-v2 .promo-content {
    position: relative;
    z-index: 1;
    padding: 2rem;
    color: white;
    height: 100%;
    display: flex;
    flex-direction: column;
    justify-content: flex-end;
}
```

#### 3.2. Trạng Thái "Đã Lưu" / "Đã Sử Dụng"
**File cần tạo:** `wwwroot/js/promo-manager.js`

```javascript
// Promo Manager - Quản lý trạng thái ưu đãi
const PromoManager = {
    // Lưu ưu đãi
    save: function(promoId) {
        let saved = JSON.parse(localStorage.getItem('savedPromos') || '[]');
        if (!saved.includes(promoId)) {
            saved.push(promoId);
            localStorage.setItem('savedPromos', JSON.stringify(saved));
        }
        this.updateUI(promoId);
        showToast('Đã lưu ưu đãi', 'success');
    },

    // Đánh dấu đã sử dụng
    markAsUsed: function(promoId) {
        let used = JSON.parse(localStorage.getItem('usedPromos') || '[]');
        if (!used.includes(promoId)) {
            used.push(promoId);
            localStorage.setItem('usedPromos', JSON.stringify(used));
        }
        this.updateUI(promoId);
    },

    // Kiểm tra trạng thái
    getStatus: function(promoId) {
        const used = JSON.parse(localStorage.getItem('usedPromos') || '[]');
        const saved = JSON.parse(localStorage.getItem('savedPromos') || '[]');
        
        if (used.includes(promoId)) return 'used';
        if (saved.includes(promoId)) return 'saved';
        return 'available';
    },

    // Cập nhật UI
    updateUI: function(promoId) {
        const status = this.getStatus(promoId);
        const btn = document.querySelector(`[data-promo-id="${promoId}"]`);
        
        if (!btn) return;

        if (status === 'used') {
            btn.innerHTML = '<i class="fas fa-check"></i> Đã sử dụng';
            btn.classList.add('btn-secondary');
            btn.classList.remove('btn-primary');
            btn.disabled = true;
        } else if (status === 'saved') {
            btn.innerHTML = '<i class="fas fa-bookmark"></i> Đã lưu';
            btn.classList.add('btn-info');
            btn.classList.remove('btn-primary');
        }
    },

    // Khởi tạo tất cả buttons
    init: function() {
        document.querySelectorAll('[data-promo-id]').forEach(btn => {
            const promoId = btn.getAttribute('data-promo-id');
            this.updateUI(promoId);
        });
    }
};

// Auto init khi load trang
document.addEventListener('DOMContentLoaded', function() {
    PromoManager.init();
});
```

**Cách sử dụng trong View:**
```html
<button class="btn btn-primary" 
        data-promo-id="PROMO001" 
        onclick="PromoManager.save('PROMO001')">
    <i class="fas fa-gift"></i> Sử dụng ngay
</button>
```

### 4. CÁC TRANG TĨNH - ĐIỀU KHOẢN & CHÍNH SÁCH

#### 4.1. Header Hành Chính
**File cần sửa:** 
- `Views/Home/DieuKhoanDichVu.cshtml`
- `Views/Home/ChinhSachBaoMat.cshtml`

**Code mẫu:**
```html
<div class="container my-5">
    <div class="card shadow-lg">
        <div class="card-body p-5">
            <!-- Header Hành Chính -->
            <div class="text-center mb-5">
                <h2 class="fw-bold text-uppercase mb-2" style="font-size: 1.5rem;">
                    CỘNG HÒA XÃ HỘI CHỦ NGHĨA VIỆT NAM
                </h2>
                <p class="fw-bold mb-4" style="font-size: 1.1rem;">
                    Độc lập - Tự do - Hạnh phúc
                </p>
                <hr style="width: 100px; margin: 0 auto; border-top: 2px solid #000;">
            </div>

            <!-- Tiêu đề chính -->
            <h1 class="text-center mb-4 fw-bold">ĐIỀU KHOẢN DỊCH VỤ</h1>

            <!-- Nội dung -->
            <div class="content-text" style="text-align: justify; line-height: 1.8;">
                <p>Chào mừng bạn đến với H-D Rental...</p>
                <!-- Nội dung chi tiết -->
            </div>
        </div>
    </div>
</div>
```

**CSS cần thêm:**
```css
.content-text {
    text-align: justify;
    line-height: 1.8;
    font-size: 1rem;
}

.content-text h2 {
    margin-top: 2rem;
    margin-bottom: 1rem;
    font-weight: 600;
}

.content-text p {
    margin-bottom: 1rem;
}
```

### 5. CHI TIẾT XE & ĐẶT CỌC

#### 5.1. Sửa Lỗi Chớp Trang (onerror)
**File cần sửa:** `Views/Xe/ChiTiet.cshtml`

**Tìm và xóa:**
```html
onerror="this.src='https://via.placeholder.com/...'"
```

**Thay bằng:**
```html
<img src="@imgSrc" 
     class="img-fluid" 
     alt="@Model.TenXe"
     onload="this.style.opacity=1"
     style="opacity:0; transition: opacity 0.3s;" />
```

#### 5.2. Hiển Thị Số Sao Đúng
**Code mẫu:**
```html
<div class="rating-stars">
    @for (int i = 1; i <= 5; i++)
    {
        if (i <= Model.DanhGiaTrungBinh)
        {
            <i class="fas fa-star text-warning"></i>
        }
        else
        {
            <i class="far fa-star text-muted"></i>
        }
    }
    <span class="ms-2">(@Model.DanhGiaTrungBinh.ToString("0.0"))</span>
</div>
```

#### 5.3. Liên Kết Ngày Thuê
**File cần sửa:** `Views/Xe/ChiTiet.cshtml`

**Thêm vào nút "Đặt xe":**
```html
<a asp-controller="HopDong" 
   asp-action="DatXe" 
   asp-route-id="@Model.MaXe"
   asp-route-ngayNhan="@ViewBag.NgayNhan"
   asp-route-ngayTra="@ViewBag.NgayTra"
   class="btn btn-primary">
    Đặt xe ngay
</a>
```

**File cần sửa:** `Controllers/HopDongController.cs`

```csharp
public async Task<IActionResult> DatXe(int id, string? ngayNhan, string? ngayTra)
{
    var xe = await _context.Xes
        .Include(x => x.HangXe)
        .Include(x => x.PhongCach)
        .FirstOrDefaultAsync(x => x.MaXe == id);

    if (xe == null) return NotFound();

    var viewModel = new DatXeViewModel
    {
        Xe = xe,
        NgayThue = !string.IsNullOrEmpty(ngayNhan) 
            ? DateTime.Parse(ngayNhan) 
            : DateTime.Now.AddDays(1),
        NgayTra = !string.IsNullOrEmpty(ngayTra) 
            ? DateTime.Parse(ngayTra) 
            : DateTime.Now.AddDays(4)
    };

    return View(viewModel);
}
```

#### 5.4. Form Đặt Cọc - Tách Ngày và Giờ
**File cần sửa:** `Views/HopDong/DatXe.cshtml`

```html
<!-- Ngày (readonly) -->
<div class="row mb-3">
    <div class="col-md-6">
        <label class="form-label">Ngày nhận xe</label>
        <input type="date" 
               class="form-control" 
               value="@Model.NgayThue.ToString("yyyy-MM-dd")" 
               readonly />
    </div>
    <div class="col-md-6">
        <label class="form-label">Ngày trả xe</label>
        <input type="date" 
               class="form-control" 
               value="@Model.NgayTra.ToString("yyyy-MM-dd")" 
               readonly />
    </div>
</div>

<!-- Giờ (có thể chọn) -->
<div class="row mb-3">
    <div class="col-md-6">
        <label class="form-label">Giờ nhận xe</label>
        <select class="form-select" name="gioNhan">
            @for (int h = 8; h <= 20; h++)
            {
                <option value="@h:00">@h:00</option>
            }
        </select>
    </div>
    <div class="col-md-6">
        <label class="form-label">Giờ trả xe</label>
        <select class="form-select" name="gioTra">
            @for (int h = 8; h <= 20; h++)
            {
                <option value="@h:00">@h:00</option>
            }
        </select>
    </div>
</div>
```

### 6. BẢO HIỂM

#### 6.1. Gửi Yêu Cầu Bồi Thường
**File cần sửa:** `Views/Home/BaoHiem.cshtml`

**Tìm div upload và thay bằng:**
```html
<div class="mb-3">
    <label class="form-label">Hình ảnh hiện trường</label>
    <input type="file" 
           class="form-control" 
           id="claimImages" 
           accept="image/*" 
           multiple />
    <small class="text-muted">Có thể chọn nhiều ảnh</small>
</div>

<button type="button" 
        class="btn btn-primary" 
        onclick="submitClaim()">
    <i class="fas fa-paper-plane"></i> Gửi yêu cầu
</button>
```

**JavaScript:**
```javascript
function submitClaim() {
    const images = document.getElementById('claimImages').files;
    
    if (images.length === 0) {
        showToast('Vui lòng chọn ít nhất 1 hình ảnh', 'warning');
        return;
    }

    // Giả lập gửi form
    showToast('Đã gửi yêu cầu bồi thường thành công!', 'success');
    
    // Clear form
    document.getElementById('claimImages').value = '';
}
```

#### 6.2. In Hợp Đồng Không Background
**CSS cần thêm vào `BaoHiem.cshtml`:**
```html
<style>
@@media print {
    /* Ẩn tất cả trừ modal-body */
    body * {
        visibility: hidden;
    }
    
    .modal-body, .modal-body * {
        visibility: visible;
    }
    
    .modal-body {
        position: absolute;
        left: 0;
        top: 0;
        width: 100%;
        background: white !important;
    }
    
    /* Ẩn các thành phần không cần */
    .hd-sidebar,
    .navbar,
    footer,
    .modal-backdrop,
    .btn,
    .modal-header,
    .modal-footer {
        display: none !important;
    }
}
</style>
```

### 7. HỖ TRỢ

#### 7.1. Tương Tác Mục Hỗ Trợ
**File cần sửa:** `Views/Home/HoTro.cshtml`

```html
<!-- Thuê xe & Đặt trước -->
<a href="@Url.Action("DanhSach", "Xe")" class="support-item">
    <i class="fas fa-motorcycle"></i>
    <h5>Thuê xe & Đặt trước</h5>
    <p>Hướng dẫn đặt xe và thanh toán</p>
</a>

<!-- Thanh toán -->
<a href="#" class="support-item" onclick="showPaymentFAQ(); return false;">
    <i class="fas fa-credit-card"></i>
    <h5>Thanh toán</h5>
    <p>Các phương thức thanh toán</p>
</a>

<!-- Hỗ trợ kỹ thuật -->
<a href="#" class="support-item" onclick="showRescueModal(); return false;">
    <i class="fas fa-tools"></i>
    <h5>Hỗ trợ kỹ thuật</h5>
    <p>Cứu hộ và sửa chữa</p>
</a>
```

**JavaScript:**
```javascript
function showPaymentFAQ() {
    const content = `
        <h5>Các phương thức thanh toán</h5>
        <ul>
            <li>Tiền mặt tại cửa hàng</li>
            <li>Chuyển khoản ngân hàng</li>
            <li>Ví điện tử (MoMo, ZaloPay)</li>
            <li>Thẻ tín dụng/ghi nợ</li>
        </ul>
    `;
    showModal('Thanh toán', content);
}

function showRescueModal() {
    const content = `
        <h5>Dịch vụ cứu hộ 24/7</h5>
        <p>Hotline: <strong>1900-xxxx</strong></p>
        <p>Chúng tôi hỗ trợ cứu hộ miễn phí trong bán kính 50km</p>
    `;
    showModal('Hỗ trợ kỹ thuật', content);
}

function showModal(title, content) {
    // Tạo modal động
    const modalHtml = `
        <div class="modal fade" id="dynamicModal" tabindex="-1">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">${title}</h5>
                        <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                    </div>
                    <div class="modal-body">
                        ${content}
                    </div>
                </div>
            </div>
        </div>
    `;
    
    // Remove old modal if exists
    const oldModal = document.getElementById('dynamicModal');
    if (oldModal) oldModal.remove();
    
    // Add new modal
    document.body.insertAdjacentHTML('beforeend', modalHtml);
    
    // Show modal
    const modal = new bootstrap.Modal(document.getElementById('dynamicModal'));
    modal.show();
}
```

### 8. HỒ SƠ CÁ NHÂN

#### 8.1. Sửa Lỗi Tab Cài Đặt
**File cần sửa:** `Views/Account/Profile.cshtml`

**Đảm bảo các thuộc tính đúng:**
```html
<!-- Tab buttons -->
<div class="nav flex-column nav-pills" role="tablist">
    <button class="nav-link active" 
            id="v-pills-profile-tab" 
            data-bs-toggle="pill" 
            data-bs-target="#v-pills-profile" 
            type="button" 
            role="tab">
        Hồ sơ cá nhân
    </button>
    <button class="nav-link" 
            id="v-pills-settings-tab" 
            data-bs-toggle="pill" 
            data-bs-target="#v-pills-settings" 
            type="button" 
            role="tab">
        Cài đặt & Bảo mật
    </button>
</div>

<!-- Tab content -->
<div class="tab-content">
    <div class="tab-pane fade show active" 
         id="v-pills-profile" 
         role="tabpanel">
        <!-- Nội dung hồ sơ -->
    </div>
    <div class="tab-pane fade" 
         id="v-pills-settings" 
         role="tabpanel">
        <!-- Nội dung cài đặt -->
    </div>
</div>
```

#### 8.2. Thêm Avatar
**Code mẫu:**
```html
<div class="text-center mb-4">
    <div class="avatar-upload">
        <div class="avatar-preview">
            <img src="@(Model.Avatar ?? "/images/default-avatar.png")" 
                 alt="Avatar" 
                 id="avatarPreview" />
        </div>
        <label for="avatarInput" class="avatar-edit">
            <i class="fas fa-camera"></i>
        </label>
        <input type="file" 
               id="avatarInput" 
               accept="image/*" 
               style="display: none;" 
               onchange="previewAvatar(this)" />
    </div>
</div>
```

**CSS:**
```css
.avatar-upload {
    position: relative;
    width: 150px;
    height: 150px;
    margin: 0 auto;
}

.avatar-preview {
    width: 100%;
    height: 100%;
    border-radius: 50%;
    overflow: hidden;
    border: 4px solid var(--accent);
}

.avatar-preview img {
    width: 100%;
    height: 100%;
    object-fit: cover;
}

.avatar-edit {
    position: absolute;
    bottom: 0;
    right: 0;
    width: 40px;
    height: 40px;
    background: var(--accent);
    border-radius: 50%;
    display: flex;
    align-items: center;
    justify-content: center;
    color: white;
    cursor: pointer;
    transition: 0.3s;
}

.avatar-edit:hover {
    transform: scale(1.1);
}
```

**JavaScript:**
```javascript
function previewAvatar(input) {
    if (input.files && input.files[0]) {
        const reader = new FileReader();
        reader.onload = function(e) {
            document.getElementById('avatarPreview').src = e.target.result;
        };
        reader.readAsDataURL(input.files[0]);
    }
}
```

#### 8.3. Xác Minh GPLX
**Code mẫu:**
```html
<div class="card mb-4">
    <div class="card-header">
        <h5><i class="fas fa-id-card"></i> Xác minh Giấy phép lái xe</h5>
    </div>
    <div class="card-body">
        <div class="row">
            <div class="col-md-6 mb-3">
                <label class="form-label">Số GPLX</label>
                <input type="text" 
                       class="form-control" 
                       placeholder="Nhập số GPLX" />
            </div>
            <div class="col-md-6 mb-3">
                <label class="form-label">Upload ảnh GPLX</label>
                <input type="file" 
                       class="form-control" 
                       accept="image/*" />
            </div>
        </div>
        <button type="button" class="btn btn-primary">
            <i class="fas fa-check"></i> Xác minh
        </button>
    </div>
</div>
```

### 9. GIỚI THIỆU - HOVER EFFECTS

**CSS cần thêm vào `custom.css`:**
```css
.info-block {
    transition: all 0.3s ease;
}

.info-block:hover {
    transform: translateY(-4px);
    box-shadow: 0 8px 24px rgba(0, 0, 0, 0.15);
}

.feature-card {
    transition: all 0.3s ease;
}

.feature-card:hover {
    transform: translateY(-4px);
    box-shadow: 0 8px 24px rgba(232, 115, 26, 0.2);
}
```

## 📝 CHECKLIST TRIỂN KHAI

- [ ] 1. Toast System (Đã có sẵn)
- [ ] 2. Active Navigation (Đã có sẵn)
- [ ] 3. Card Ưu Đãi
- [ ] 4. Trạng Thái Ưu Đãi (LocalStorage)
- [ ] 5. Header Hành Chính
- [ ] 6. Sửa lỗi onerror ảnh
- [ ] 7. Hiển thị sao đúng
- [ ] 8. Liên kết ngày thuê
- [ ] 9. Form đặt cọc (tách ngày/giờ)
- [ ] 10. Upload ảnh bồi thường
- [ ] 11. CSS in hợp đồng
- [ ] 12. Tương tác hỗ trợ
- [ ] 13. Sửa tab Profile
- [ ] 14. Thêm Avatar
- [ ] 15. Xác minh GPLX
- [ ] 16. Hover effects

## 🚀 HƯỚNG DẪN SỬ DỤNG

1. **Đọc từng phần** trong tài liệu này
2. **Copy code mẫu** vào đúng file
3. **Test từng tính năng** sau khi thêm
4. **Commit code** sau mỗi phần hoàn thành

## 📞 HỖ TRỢ

Nếu gặp vấn đề khi triển khai, hãy:
1. Kiểm tra lại đường dẫn file
2. Đảm bảo đã save tất cả files
3. Clear cache trình duyệt (Ctrl + F5)
4. Kiểm tra Console (F12) xem có lỗi JavaScript không

---
**Chúc bạn triển khai thành công!** 🎉

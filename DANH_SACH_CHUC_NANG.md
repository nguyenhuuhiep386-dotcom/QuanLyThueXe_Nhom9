# DANH SÁCH CHỨC NĂNG HỆ THỐNG

## I. CHỨC NĂNG DÀNH CHO ADMIN

### 1. Dashboard (Trang tổng quan)
- ✅ Hiển thị thống kê tổng quan:
  - Tổng số xe (Còn trống, Đang thuê, Bảo dưỡng)
  - Doanh thu tháng/năm
  - Tổng số hợp đồng (Chờ xác nhận, Đang thuê)
  - Tổng số khách hàng
- ✅ Danh sách hợp đồng gần đây
- ✅ Danh sách xe phổ biến
- ✅ Thông báo mới

### 2. Quản lý Xe
- ✅ Xem danh sách xe
- ✅ Tìm kiếm xe theo tên, biển số
- ✅ Lọc xe theo trạng thái
- ✅ Thêm xe mới
  - Thông tin cơ bản (tên, biển số, hãng, phong cách)
  - Giá thuê, năm sản xuất, dung tích
  - Upload nhiều hình ảnh
  - Mô tả chi tiết
- ✅ Sửa thông tin xe
- ✅ Xóa xe (kiểm tra ràng buộc với hợp đồng)
- ✅ Xem chi tiết xe
  - Thông tin đầy đủ
  - Lịch sử thuê
  - Đánh giá từ khách hàng

### 3. Quản lý Khách hàng
- ✅ Xem danh sách khách hàng
- ✅ Tìm kiếm khách hàng (tên, SĐT, email)
- ✅ Thêm khách hàng mới
  - Thông tin cá nhân
  - Số GPLX, giấy tờ tùy thân
  - Địa chỉ, tỉnh/thành
- ✅ Sửa thông tin khách hàng
- ✅ Xem chi tiết khách hàng
  - Thông tin đầy đủ
  - Lịch sử thuê xe
  - Đánh giá đã viết

### 4. Quản lý Hợp đồng
- ✅ Xem danh sách hợp đồng
- ✅ Lọc theo trạng thái (Chờ xác nhận, Đang thuê, Đã trả, Đã hủy)
- ✅ Lọc theo khoảng thời gian
- ✅ Tạo hợp đồng mới
  - Chọn xe, khách hàng
  - Ngày thuê, ngày trả
  - Tự động tính giá (bao gồm phụ phí mùa)
- ✅ Xem chi tiết hợp đồng
  - Thông tin đầy đủ
  - Lịch sử thanh toán
  - Tài liệu đính kèm
- ✅ Xác nhận hợp đồng
  - Chuyển trạng thái từ "Chờ xác nhận" → "Đang thuê"
  - Cập nhật trạng thái xe
- ✅ Hủy hợp đồng
  - Nhập lý do hủy
  - Cập nhật trạng thái xe
- ✅ Trả xe
  - Nhập ngày trả thực tế
  - Tự động tính phụ phí trễ hạn
  - Cập nhật trạng thái xe

### 5. Quản lý Bảo dưỡng
- ⚠️ Xem danh sách lịch bảo dưỡng
- ⚠️ Thêm lịch bảo dưỡng mới
- ⚠️ Cập nhật trạng thái bảo dưỡng
- ⚠️ Xem chi phí bảo dưỡng

### 6. Quản lý Khuyến mãi
- ⚠️ Xem danh sách khuyến mãi
- ⚠️ Tạo mã khuyến mãi mới
  - Loại giảm giá (%, số tiền cố định)
  - Thời gian áp dụng
  - Điều kiện tối thiểu
  - Số lần sử dụng
- ⚠️ Sửa/Xóa khuyến mãi
- ⚠️ Xem lịch sử sử dụng

### 7. Quản lý Đánh giá
- ⚠️ Xem danh sách đánh giá
- ⚠️ Ẩn/Hiện đánh giá
- ⚠️ Xóa đánh giá vi phạm

### 8. Báo cáo & Thống kê
- ⚠️ Báo cáo doanh thu theo tháng/năm
- ⚠️ Thống kê xe được thuê nhiều nhất
- ⚠️ Thống kê khách hàng thân thiết
- ⚠️ Báo cáo tình trạng xe
- ⚠️ Xuất báo cáo Excel/PDF

## II. CHỨC NĂNG DÀNH CHO KHÁCH HÀNG

### 1. Trang chủ
- ✅ Hiển thị xe phổ biến
- ✅ Hiển thị khuyến mãi đang có
- ✅ Banner giới thiệu
- ✅ Các tính năng nổi bật

### 2. Xem danh sách xe
- ✅ Hiển thị tất cả xe còn trống
- ✅ Tìm kiếm xe theo tên
- ✅ Lọc theo hãng xe
- ✅ Lọc theo phong cách
- ✅ Lọc theo khoảng giá
- ✅ Hiển thị giá, đánh giá, số lần thuê

### 3. Xem chi tiết xe
- ✅ Hình ảnh xe (nhiều ảnh, có thể xem lớn)
- ✅ Thông tin chi tiết (hãng, phong cách, năm SX, dung tích)
- ✅ Giá thuê/ngày
- ✅ Đánh giá từ khách hàng khác
- ✅ Xe tương tự
- ✅ Nút "Đặt xe ngay"

### 4. Đặt xe
- ✅ Chọn ngày thuê, ngày trả
- ✅ Nhập mã khuyến mãi (nếu có)
- ✅ Xem tổng tiền (bao gồm phụ phí mùa, giảm giá)
- ✅ Xác nhận đặt xe
- ✅ Upload giấy tờ (GPLX, CMND/CCCD)

### 5. Quản lý hợp đồng của tôi
- ✅ Xem danh sách hợp đồng
- ✅ Lọc theo trạng thái
- ✅ Xem chi tiết hợp đồng
- ✅ Hủy hợp đồng (nếu chưa xác nhận)
- ⚠️ Xem lịch sử thanh toán

### 6. Đánh giá xe
- ✅ Đánh giá sau khi trả xe
- ✅ Chọn số sao (1-5)
- ✅ Viết nhận xét

### 7. Hồ sơ cá nhân
- ✅ Xem thông tin cá nhân
- ✅ Cập nhật thông tin
- ⚠️ Đổi mật khẩu
- ⚠️ Upload/Cập nhật GPLX

### 8. Thông báo
- ⚠️ Nhận thông báo về hợp đồng
- ⚠️ Nhận thông báo khuyến mãi
- ⚠️ Đánh dấu đã đọc

## III. CHỨC NĂNG CHUNG

### 1. Xác thực & Phân quyền
- ✅ Đăng ký tài khoản mới
- ✅ Đăng nhập
- ✅ Đăng xuất
- ✅ Phân quyền theo vai trò (Admin/Khách hàng)
- ⚠️ Quên mật khẩu
- ⚠️ Đổi mật khẩu

### 2. Thanh toán
- ⚠️ Thanh toán đặt cọc
- ⚠️ Thanh toán cuối
- ⚠️ Nhiều phương thức (Tiền mặt, Chuyển khoản, Ví điện tử)
- ⚠️ Lịch sử thanh toán

### 3. Upload file
- ✅ Upload hình ảnh xe
- ⚠️ Upload giấy tờ khách hàng
- ⚠️ Upload tài liệu hợp đồng

## CHỨC NĂNG TỰ ĐỘNG

### 1. Tính toán giá
- ✅ Tính giá thuê theo số ngày
- ✅ Áp dụng hệ số phụ phí mùa
- ⚠️ Áp dụng mã khuyến mãi
- ✅ Tính phụ phí trễ hạn

### 2. Cập nhật trạng thái
- ✅ Cập nhật trạng thái xe khi xác nhận hợp đồng
- ✅ Cập nhật trạng thái xe khi trả xe
- ✅ Cập nhật trạng thái xe khi hủy hợp đồng

### 3. Thông báo
- ⚠️ Thông báo hợp đồng sắp hết hạn
- ⚠️ Thông báo xe cần bảo dưỡng
- ⚠️ Thông báo khuyến mãi mới

## GHI CHÚ

✅ = Đã hoàn thành
⚠️ = Chưa hoàn thành (cần bổ sung)
❌ = Chưa triển khai

## CÁC CHỨC NĂNG CẦN BỔ SUNG

1. **Ưu tiên cao:**
   - Đặt xe (khách hàng)
   - Quản lý hợp đồng của tôi
   - Hồ sơ cá nhân
   - Upload giấy tờ

2. **Ưu tiên trung bình:**
   - Quản lý bảo dưỡng
   - Quản lý khuyến mãi
   - Quản lý đánh giá
   - Thanh toán

3. **Ưu tiên thấp:**
   - Báo cáo & Thống kê
   - Quên mật khẩu
   - Thông báo tự động
   - Xuất báo cáo

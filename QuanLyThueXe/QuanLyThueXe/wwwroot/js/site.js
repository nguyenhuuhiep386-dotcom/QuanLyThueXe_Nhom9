// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// ===== GLOBAL NOTIFICATION FUNCTIONS =====
function showToast(message, type = 'success') {
    const bg = type === 'error' ? '#dc3545' : type === 'warning' ? '#ffc107' : '#e8731a';
    const icon = type === 'error' ? 'fa-exclamation-circle' : type === 'warning' ? 'fa-exclamation-triangle' : 'fa-check-circle';
    const id = 'toast_' + Date.now();
    const html = `
        <div id="${id}" style="position:fixed; bottom:24px; right:24px; z-index:9999; min-width:320px; max-width:400px;
                    background:${bg}; color:#fff; border-radius:12px; padding:16px 20px; display:flex; 
                    align-items:center; gap:12px; box-shadow:0 8px 24px rgba(0,0,0,0.4);
                    animation:slideInRight 0.3s ease; font-size:14px; font-family:'Inter',sans-serif;">
            <i class="fas ${icon}" style="font-size:20px; flex-shrink:0;"></i>
            <span style="flex:1;">${message}</span>
            <button onclick="document.getElementById('${id}').remove()" 
                    style="background:none;border:none;color:#fff;cursor:pointer;padding:0;font-size:18px;line-height:1;">&times;</button>
        </div>
        <style>
            @keyframes slideInRight { from { transform:translateX(120%); opacity:0; } to { transform:translateX(0); opacity:1; } }
        </style>`;
    document.body.insertAdjacentHTML('beforeend', html);
    setTimeout(() => { const el = document.getElementById(id); if(el) el.style.animation='none', el.style.opacity='0', el.style.transition='opacity 0.4s', setTimeout(()=>el.remove(), 400); }, 4000);
}

function showNotificationModal(title, message, type = 'success') {
    const icon = type === 'error' ? 'fa-times-circle' : type === 'warning' ? 'fa-exclamation-triangle' : 'fa-check-circle';
    const color = type === 'error' ? '#dc3545' : type === 'warning' ? '#ffc107' : '#e8731a';
    const id = 'notifModal_' + Date.now();
    const html = `
        <div id="${id}" style="position:fixed;inset:0;z-index:10000;display:flex;align-items:center;justify-content:center;background:rgba(0,0,0,0.6);">
            <div style="background:#242424;border:1px solid #333;border-radius:16px;padding:40px;text-align:center;max-width:400px;width:90%;box-shadow:0 16px 48px rgba(0,0,0,0.6);">
                <i class="fas ${icon}" style="font-size:56px;color:${color};margin-bottom:20px;display:block;"></i>
                <h4 style="font-size:20px;font-weight:700;margin-bottom:12px;color:#fff;">${title}</h4>
                <p style="font-size:14px;color:#a0a0a0;margin-bottom:28px;line-height:1.6;">${message}</p>
                <button onclick="document.getElementById('${id}').remove()" 
                        style="background:${color};color:#fff;border:none;padding:12px 32px;border-radius:8px;font-size:14px;font-weight:600;cursor:pointer;">Đóng</button>
            </div>
        </div>`;
    document.body.insertAdjacentHTML('beforeend', html);
}

// ===== GLOBAL SEARCH =====
document.addEventListener('DOMContentLoaded', function() {
    const searchInput = document.getElementById('globalSearchInput');
    const searchResults = document.getElementById('globalSearchResults');

    if(searchInput && searchResults) {
        const defaultLinks = `
            <div style="padding: 4px 12px; font-size:11px; color:var(--text-muted); text-transform:uppercase;">Gợi ý</div>
            <a href="/Home/BaoHiem" class="search-item" style="display:block; padding:8px 16px; color:var(--text-primary); text-decoration:none; font-size:13px; transition:0.2s;" onmouseover="this.style.background='var(--border-color)'" onmouseout="this.style.background='transparent'"><i class="fas fa-shield-alt" style="width:20px; color:var(--text-muted);"></i> Bảo hiểm</a>
            <a href="/Home/HoTro" class="search-item" style="display:block; padding:8px 16px; color:var(--text-primary); text-decoration:none; font-size:13px; transition:0.2s;" onmouseover="this.style.background='var(--border-color)'" onmouseout="this.style.background='transparent'"><i class="fas fa-headset" style="width:20px; color:var(--text-muted);"></i> Hỗ trợ</a>
            <a href="/Home/ChinhSachBaoMat" class="search-item" style="display:block; padding:8px 16px; color:var(--text-primary); text-decoration:none; font-size:13px; transition:0.2s;" onmouseover="this.style.background='var(--border-color)'" onmouseout="this.style.background='transparent'"><i class="fas fa-user-shield" style="width:20px; color:var(--text-muted);"></i> Chính sách bảo mật</a>
            <a href="/Home/DieuKhoanDichVu" class="search-item" style="display:block; padding:8px 16px; color:var(--text-primary); text-decoration:none; font-size:13px; transition:0.2s;" onmouseover="this.style.background='var(--border-color)'" onmouseout="this.style.background='transparent'"><i class="fas fa-file-contract" style="width:20px; color:var(--text-muted);"></i> Điều khoản dịch vụ</a>
            <a href="/Home/GioiThieu" class="search-item" style="display:block; padding:8px 16px; color:var(--text-primary); text-decoration:none; font-size:13px; transition:0.2s;" onmouseover="this.style.background='var(--border-color)'" onmouseout="this.style.background='transparent'"><i class="fas fa-info-circle" style="width:20px; color:var(--text-muted);"></i> Giới thiệu</a>
        `;

        searchInput.addEventListener('focus', function() {
            if(!this.value.trim()) {
                searchResults.innerHTML = defaultLinks;
            }
            searchResults.style.display = 'block';
        });

        searchInput.addEventListener('input', function() {
            const val = this.value.trim();
            if(!val) {
                searchResults.innerHTML = defaultLinks;
                return;
            }
            
            fetch('/Xe/SearchApi?q=' + encodeURIComponent(val))
                .then(res => res.json())
                .then(data => {
                    if(data.length === 0) {
                        searchResults.innerHTML = `<div style="padding:12px 16px; font-size:13px; color:var(--text-muted);">Không tìm thấy kết quả cho "${val}"</div>`;
                        return;
                    }
                    let html = `<div style="padding: 4px 12px; font-size:11px; color:var(--text-muted); text-transform:uppercase;">Xe tìm thấy</div>`;
                    data.forEach(item => {
                        html += `<a href="/Xe/ChiTiet/${item.maXe}" class="search-item" style="display:block; padding:8px 16px; color:var(--text-primary); text-decoration:none; font-size:13px; transition:0.2s;" onmouseover="this.style.background='var(--border-color)'" onmouseout="this.style.background='transparent'">
                                    <i class="fas fa-motorcycle" style="width:20px; color:var(--accent);"></i> ${item.tenXe}
                                 </a>`;
                    });
                    searchResults.innerHTML = html;
                })
                .catch(() => {
                    searchResults.innerHTML = defaultLinks;
                });
        });

        document.addEventListener('click', function(e) {
            if(!searchInput.contains(e.target) && !searchResults.contains(e.target)) {
                searchResults.style.display = 'none';
            }
        });
    }
});

// ===== HELPER FUNCTIONS FOR SUPPORT PAGE =====
function showPaymentFAQ() {
    const content = `
        <h5>Các phương thức thanh toán</h5>
        <ul style="text-align: left; line-height: 2;">
            <li><i class="fas fa-money-bill-wave text-success"></i> Tiền mặt tại cửa hàng</li>
            <li><i class="fas fa-university text-primary"></i> Chuyển khoản ngân hàng</li>
            <li><i class="fas fa-wallet text-warning"></i> Ví điện tử (MoMo, ZaloPay)</li>
            <li><i class="fas fa-credit-card text-info"></i> Thẻ tín dụng/ghi nợ</li>
        </ul>
        <hr>
        <p style="font-size: 13px; color: var(--text-muted);">
            <i class="fas fa-info-circle"></i> Tất cả giao dịch đều được bảo mật và mã hóa
        </p>
    `;
    showModal('Thanh toán', content);
}

function showRescueModal() {
    const content = `
        <h5>Dịch vụ cứu hộ 24/7</h5>
        <p style="font-size: 16px; margin: 20px 0;">
            <i class="fas fa-phone-alt text-success"></i> 
            Hotline: <strong style="color: var(--accent);">1900-xxxx</strong>
        </p>
        <hr>
        <p style="font-size: 14px; line-height: 1.8;">
            <i class="fas fa-check-circle text-success"></i> Hỗ trợ cứu hộ miễn phí trong bán kính 50km<br>
            <i class="fas fa-check-circle text-success"></i> Đội ngũ kỹ thuật viên chuyên nghiệp<br>
            <i class="fas fa-check-circle text-success"></i> Thời gian phản hồi trung bình: 30 phút
        </p>
    `;
    showModal('Hỗ trợ kỹ thuật', content);
}

function showModal(title, content) {
    // Tạo modal động
    const modalHtml = `
        <div class="modal fade" id="dynamicModal" tabindex="-1">
            <div class="modal-dialog modal-dialog-centered">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">${title}</h5>
                        <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                    </div>
                    <div class="modal-body">
                        ${content}
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Đóng</button>
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

// ===== HELPER FUNCTION FOR INSURANCE CLAIM =====
function submitClaim() {
    const images = document.getElementById('claimImages');
    
    if (!images) {
        showToast('Không tìm thấy form upload', 'error');
        return;
    }
    
    if (images.files.length === 0) {
        showToast('Vui lòng chọn ít nhất 1 hình ảnh', 'warning');
        return;
    }

    // Giả lập gửi form
    showToast('Đã gửi yêu cầu bồi thường thành công!', 'success');
    
    // Clear form
    images.value = '';
}

// ===== HELPER FUNCTION FOR AVATAR PREVIEW =====
function previewAvatar(input) {
    if (input.files && input.files[0]) {
        const reader = new FileReader();
        reader.onload = function(e) {
            const preview = document.getElementById('avatarPreview');
            if (preview) {
                preview.src = e.target.result;
            }
        };
        reader.readAsDataURL(input.files[0]);
    }
}

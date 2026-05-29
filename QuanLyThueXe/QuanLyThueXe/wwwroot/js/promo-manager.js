// Promo Manager - Quản lý trạng thái ưu đãi
const PromoManager = {
    // Lưu ưu đãi
    save: function(promoId) {
        let saved = JSON.parse(localStorage.getItem('savedPromos') || '[]');
        if (!saved.includes(promoId)) {
            saved.push(promoId);
            localStorage.setItem('savedPromos', JSON.stringify(saved));
            
            // Notification
            if (typeof showToast === 'function') {
                showToast('Đã lưu vào trong khuyến mãi của bạn', 'success');
            } else {
                Swal.fire({
                    icon: 'success',
                    title: 'Lưu thành công',
                    text: 'Đã lưu vào trong khuyến mãi của bạn',
                    confirmButtonColor: '#ea580c'
                });
            }
        }
        this.updateUI(promoId);
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
    getStatus: function(promoId, expiryDateStr) {
        // Check expiry first
        if (expiryDateStr) {
            // Chuẩn hóa chuỗi ngày dạng DD/MM/YYYY thành YYYY-MM-DD
            let parts = expiryDateStr.split('/');
            let validDateStr = expiryDateStr;
            if (parts.length === 3) {
                validDateStr = `${parts[2]}-${parts[1]}-${parts[0]}`;
            }
            const expiryDate = new Date(validDateStr);
            if (new Date() > expiryDate) {
                return 'expired';
            }
        }
        
        const used = JSON.parse(localStorage.getItem('usedPromos') || '[]');
        const saved = JSON.parse(localStorage.getItem('savedPromos') || '[]');
        
        if (used.includes(promoId)) return 'used';
        if (saved.includes(promoId)) return 'saved';
        return 'available';
    },

    // Cập nhật UI
    updateUI: function(promoId) {
        document.querySelectorAll(`[data-promo-id="${promoId}"]`).forEach(btn => {
            const expiry = btn.getAttribute('data-promo-expiry');
            const status = this.getStatus(promoId, expiry);
            
            if (status === 'expired') {
                btn.innerHTML = '<i class="fas fa-times-circle"></i> Đã hết hạn';
                btn.classList.add('btn-secondary');
                btn.classList.remove('btn-hd', 'btn-hd-outline', 'btn-info', 'btn-primary');
                btn.disabled = true;
                btn.style.opacity = '0.7';
                btn.style.cursor = 'not-allowed';
            } else if (status === 'used') {
                btn.innerHTML = '<i class="fas fa-check"></i> Đã sử dụng';
                btn.classList.add('btn-secondary');
                btn.classList.remove('btn-hd', 'btn-hd-outline', 'btn-info', 'btn-primary');
                btn.disabled = true;
                btn.style.opacity = '0.7';
                btn.style.cursor = 'not-allowed';
            } else if (status === 'saved') {
                btn.innerHTML = '<i class="fas fa-bookmark"></i> Đã lưu';
                btn.classList.add('btn-info');
                btn.classList.remove('btn-hd', 'btn-hd-outline', 'btn-secondary', 'btn-primary');
                btn.style.background = '#60a5fa'; // Blue
                btn.style.borderColor = '#60a5fa';
                btn.style.color = '#fff';
            }
        });
    },

    // Khởi tạo tất cả buttons
    init: function() {
        document.querySelectorAll('[data-promo-id]').forEach(btn => {
            const promoId = btn.getAttribute('data-promo-id');
            this.updateUI(promoId);
            
            // Add event listener if not already added
            if (!btn.hasAttribute('data-initialized')) {
                btn.addEventListener('click', function() {
                    const status = PromoManager.getStatus(promoId, btn.getAttribute('data-promo-expiry'));
                    if (status === 'available') {
                        PromoManager.save(promoId);
                    }
                });
                btn.setAttribute('data-initialized', 'true');
            }
        });
    }
};

// Auto init khi load trang
document.addEventListener('DOMContentLoaded', function() {
    PromoManager.init();
});

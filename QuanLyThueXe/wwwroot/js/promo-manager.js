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
            btn.classList.remove('btn-primary', 'btn-info');
            btn.disabled = true;
        } else if (status === 'saved') {
            btn.innerHTML = '<i class="fas fa-bookmark"></i> Đã lưu';
            btn.classList.add('btn-info');
            btn.classList.remove('btn-primary', 'btn-secondary');
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

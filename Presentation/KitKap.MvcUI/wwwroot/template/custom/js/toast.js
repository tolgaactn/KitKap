// ========================================
// 🔔 TOAST NOTIFICATION SİSTEMİ
// ========================================

/**
 * Toast bildirimi göster
 * @param {string} type - 'success', 'error', 'warning', 'info'
 * @param {string} message - Gösterilecek mesaj
 */
function showToast(type, message) {
    const container = $('#toast-container');
    const toastId = 'toast-' + Date.now();

    // Icon belirleme
    const icons = {
        success: 'fa-check-circle',
        error: 'fa-exclamation-circle',
        warning: 'fa-exclamation-triangle',
        info: 'fa-info-circle'
    };

    const colors = {
        success: '#10b981',
        error: '#ef4444',
        warning: '#f59e0b',
        info: '#3b82f6'
    };

    const icon = icons[type] || icons.info;
    const color = colors[type] || colors.info;

    // Toast HTML
    const toastHtml = `
        <div id="${toastId}" class="toast-notification ${type}" style="border-left-color: ${color};">
            <div class="toast-icon" style="color: ${color};">
                <i class="fas ${icon}"></i>
            </div>
            <div class="toast-content">
                <p class="toast-message">${message}</p>
            </div>
            <button class="toast-close" onclick="closeToast('${toastId}')">
                <i class="fas fa-times"></i>
            </button>
        </div>
    `;

    // Container'a ekle
    container.append(toastHtml);

    // Animasyon için timeout
    setTimeout(function () {
        $('#' + toastId).addClass('show');
    }, 100);

    // 5 saniye sonra otomatik kapat
    setTimeout(function () {
        closeToast(toastId);
    }, 5000);
}

/**
 * Toast'ı kapat
 * @param {string} toastId - Kapatılacak toast ID'si
 */
function closeToast(toastId) {
    const toast = $('#' + toastId);
    toast.removeClass('show');

    // Animasyon bitince DOM'dan sil
    setTimeout(function () {
        toast.remove();
    }, 300);
}
// ========================================
// 🛒 SEPET YÖNETİMİ - KURUMSAL STANDART
// ========================================

/**
 * Sepete ürün ekleme
 * @param {number} productId - Eklenecek ürün ID'si
 * @param {HTMLElement} buttonElement - Tıklanan buton elementi
 */
function addToCart(productId, buttonElement) {
    const $btn = $(buttonElement);
    const originalHtml = $btn.html();

    // Button disabled + loading
    $btn.prop('disabled', true).html('<i class="fas fa-spinner fa-spin"></i>');

    $.ajax({
        url: '/ShoppingCart/AddToCart',
        type: 'POST',
        data: { productId: productId },
        success: function (response) {
            if (response.success) {
                // Badge güncelle
                updateCartBadge(response.cartItemCount);

                // ✅ SADECE İLK ÜRÜN EKLENİNCE DROPDOWN YENİLE (Performans)
                if (response.cartItemCount === 1) {
                    refreshCartDropdown();
                }

                // Success feedback
                showToast('success', response.message || 'Ürün sepete eklendi');
                $btn.html('<i class="fas fa-check"></i>');
                setTimeout(() => $btn.html(originalHtml), 1500);
            } else {
                showToast('error', response.message || 'Ürün eklenemedi');
                $btn.html(originalHtml);
            }
        },
        error: function (xhr, status, error) {
            console.error('Sepete ekleme hatası:', error);
            showToast('error', 'Bir hata oluştu. Lütfen tekrar deneyin.');
            $btn.html(originalHtml);
        },
        complete: function () {
            $btn.prop('disabled', false);
        }
    });
}

/**
 * Sepetten ürün çıkarma
 * @param {number} productId - Çıkarılacak ürün ID'si
 */
function removeFromCart(productId) {
    // Onay dialogu (kurumsal standart)
    if (!confirm('Bu ürünü sepetten çıkarmak istediğinize emin misiniz?')) {
        return;
    }

    $.ajax({
        url: '/ShoppingCart/RemoveFromCart',
        type: 'POST',
        data: { productId: productId },
        success: function (response) {
            if (response.success) {
                updateCartBadge(response.cartItemCount);
                refreshCartDropdown();
                showToast('success', response.message || 'Ürün sepetten çıkarıldı');
            } else {
                showToast('error', response.message || 'İşlem başarısız');
            }
        },
        error: function (xhr, status, error) {
            console.error('Sepetten çıkarma hatası:', error);
            showToast('error', 'Bir hata oluştu.');
        }
    });
}

/**
 * Sepet dropdown içeriğini yenile
 */
function refreshCartDropdown() {
    $.ajax({
        url: '/ShoppingCart/GetCartDropdown',
        type: 'GET',
        cache: false, // ✅ Cache'i engelle (her zaman fresh data)
        success: function (html) {
            // ✅ Tüm dropdown içeriğini güncelle (daha garantili)
            $('.dropdownmenu-wrapper.custom-scrollbar').html(html);
        },
        error: function (xhr, status, error) {
            console.error('Dropdown yenileme hatası:', {
                status: xhr.status,
                error: error,
                response: xhr.responseText
            });
            // ✅ Kullanıcıya hata gösterme (sessiz fail)
        }
    });
}

/**
 * Sepet badge güncelleme
 * @param {number} count - Toplam ürün sayısı
 */
function updateCartBadge(count) {
    const $cartToggle = $('.cart-toggle');
    let $badge = $cartToggle.find('.cart-count.badge-circle');

    if (count > 0) {
        if ($badge.length === 0) {
            // Badge yoksa oluştur
            $cartToggle.find('.minicart-icon').after(
                `<span class="cart-count badge-circle">${count}</span>`
            );
            $badge = $cartToggle.find('.cart-count.badge-circle');
        } else {
            // Badge varsa güncelle
            $badge.text(count);
        }

        // Animasyon efekti
        $badge.addClass('badge-bounce');
        setTimeout(() => $badge.removeClass('badge-bounce'), 600);
        $badge.fadeIn();
    } else {
        // Sepet boşsa badge'i kaldır
        $badge.fadeOut(300, function () {
            $(this).remove();
        });
    }
}

/**
 * Toast bildirimi göster
 * @param {string} type - 'success' veya 'error'
 * @param {string} message - Gösterilecek mesaj
 */
function showToast(type, message) {
    let $container = $('#toast-container');

    // Container yoksa oluştur
    if ($container.length === 0) {
        $('body').append('<div id="toast-container" class="toast-container"></div>');
        $container = $('#toast-container');
    }

    const toastId = 'toast-' + Date.now();
    const iconClass = type === 'success' ? 'fa-check-circle' : 'fa-exclamation-circle';
    const toastClass = type === 'success' ? 'toast-success' : 'toast-error';

    const toastHtml = `
        <div id="${toastId}" class="toast ${toastClass}">
            <i class="fas ${iconClass}"></i>
            <span>${message}</span>
            <button class="toast-close" onclick="closeToast('${toastId}')">&times;</button>
        </div>
    `;

    $container.append(toastHtml);

    // 3 saniye sonra otomatik kapat
    setTimeout(() => closeToast(toastId), 3000);
}

/**
 * Toast bildirimi kapat
 * @param {string} toastId - Kapatılacak toast ID'si
 */
function closeToast(toastId) {
    $(`#${toastId}`).fadeOut(300, function () {
        $(this).remove();
    });
}

$(document).ready(function () {
    $('.alert').addClass('auto-dismiss');

    setTimeout(function () {
        $('.alert').alert('close');
    }, 5000);
});

// ========================================
// 🛒 SEPET DROPDOWN YÖNETİMİ
// ========================================

$(document).ready(function () {
    console.log('🛒 Sepet sistemi başlatılıyor...');

    // ========================================
    // TEMİZLİK: Gereksiz banner'ları kaldır
    // ========================================
    $('img[src*="banner-"]').closest('div, section, figure, a').remove();

    // ========================================
    // Bootstrap dropdown çakışmasını engelle
    // ========================================
    $('.cart-toggle').off('click.bs.dropdown');
    $('#shoppingCart').off('show.bs.dropdown hide.bs.dropdown');

    // ========================================
    // ✅ SEPET TOGGLE (Kurumsal Click-Only)
    // ========================================
    $(document).on('click', '.cart-toggle', function (e) {
        e.preventDefault();
        e.stopPropagation();
        e.stopImmediatePropagation();

        const $cart = $('.cart-dropdown-custom');
        const isOpen = $cart.hasClass('cart-open');

        console.log('🛒 Sepet:', isOpen ? 'Kapatılıyor' : 'Açılıyor');

        if (isOpen) {
            closeCart();
        } else {
            openCart();
        }

        return false;
    });

    // ========================================
    // ✅ SEPET AÇMA (İlk açılışta içeriği yenile)
    // ========================================
    function openCart() {
        $('.cart-dropdown-custom').addClass('cart-open');
        $('.cart-overlay').addClass('active');
        $('body').addClass('cart-open');

        // ✅ İlk açılışta güncel veriyi getir (Kurumsal Standart)
        refreshCartDropdown();

        console.log('✅ Sepet açıldı');
    }

    // ========================================
    // ✅ SEPET KAPAMA
    // ========================================
    function closeCart() {
        $('.cart-dropdown-custom').removeClass('cart-open');
        $('.cart-overlay').removeClass('active');
        $('body').removeClass('cart-open');
        console.log('✅ Sepet kapatıldı');
    }

    // ========================================
    // Overlay'e tıklayınca kapat
    // ========================================
    $(document).on('click', '.cart-overlay', function (e) {
        e.stopPropagation();
        closeCart();
    });

    // ========================================
    // Global close fonksiyonu (HTML'den çağrılabilir)
    // ========================================
    window.closeCart = closeCart;

    // ========================================
    // ESC tuşu ile kapat
    // ========================================
    $(document).on('keydown', function (e) {
        if (e.key === 'Escape' && $('.cart-dropdown-custom').hasClass('cart-open')) {
            closeCart();
        }
    });

    // ========================================
    // Sayfa yüklendiğinde badge kontrolü
    // ========================================
    const initialBadgeCount = parseInt($('.cart-count.badge-circle').text()) || 0;
    if (initialBadgeCount === 0) {
        $('.cart-count.badge-circle').hide();
    }

    console.log('✅ Sepet sistemi hazır!');
});

// ========================================
// 📊 PERFORMANS İZLEME (Opsiyonel - Production'da kaldırılabilir)
// ========================================
if (window.performance && console.table) {
    $(window).on('load', function () {
        console.log('⏱️ Sepet performans metrikleri:', {
            'DOM Ready': performance.timing.domContentLoadedEventEnd - performance.timing.navigationStart + 'ms',
            'Page Load': performance.timing.loadEventEnd - performance.timing.navigationStart + 'ms'
        });
    });
}
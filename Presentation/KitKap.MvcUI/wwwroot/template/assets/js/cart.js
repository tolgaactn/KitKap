// ========================================
// 🛒 SEPET FONKSİYONLARI
// ========================================

function addToCart(productId, buttonElement) {
    const $btn = $(buttonElement);
    const originalHtml = $btn.html();

    $btn.prop('disabled', true);
    $btn.html('<i class="fas fa-spinner fa-spin"></i>');

    $.ajax({
        url: '/ShoppingCart/AddToCart',
        type: 'POST',
        data: { productId: productId },
        success: function (response) {
            if (response.success) {
                updateCartBadge(response.cartItemCount);
                showToast('success', response.message);
                refreshCartDropdown();
                $btn.html('<i class="fas fa-check"></i>');
                setTimeout(() => $btn.html(originalHtml), 1500);
            } else {
                showToast('error', response.message);
                $btn.html(originalHtml);
            }
        },
        error: function () {
            showToast('error', 'Bir hata oluştu.');
            $btn.html(originalHtml);
        },
        complete: function () {
            $btn.prop('disabled', false);
        }
    });
}

function removeFromCart(productId, buttonElement) {
    $.ajax({
        url: '/ShoppingCart/RemoveFromCart',
        type: 'POST',
        data: { productId: productId },
        success: function (response) {
            if (response.success) {
                updateCartBadge(response.cartItemCount);
                refreshCartDropdown();
                showToast('success', response.message);
            } else {
                showToast('error', response.message);
            }
        },
        error: function () {
            showToast('error', 'Silme işlemi başarısız.');
        }
    });
}

function refreshCartDropdown() {
    $.ajax({
        url: '/ShoppingCart/GetCartDropdown',
        type: 'GET',
        success: function (html) {
            const $tmp = $('<div>').html(html);
            const $newProducts = $tmp.find('.dropdown-cart-products');

            if ($newProducts.length && $('.dropdownmenu-wrapper .dropdown-cart-products').length) {
                $('.dropdownmenu-wrapper .dropdown-cart-products').replaceWith($newProducts);
                return;
            }

            const $newInner = $tmp.find('.dropdownmenu-wrapper .dropdown-cart-products, .dropdown-cart-products');

            if ($newInner.length && $('.dropdownmenu-wrapper .dropdown-cart-products').length) {
                $('.dropdownmenu-wrapper .dropdown-cart-products').replaceWith($newInner);
                return;
            }

            $('.dropdownmenu-wrapper').html(html);
        },
        error: function (xhr) {
            console.error('Dropdown yenileme hatası:', xhr.responseText);
        }
    });
}

function updateCartBadge(count) {
    const $badge = $('.cart-count');
    $badge.text(count);
    $badge.addClass('badge-bounce');
    setTimeout(() => $badge.removeClass('badge-bounce'), 600);

    if (count === 0) {
        $badge.fadeOut();
    } else {
        $badge.fadeIn();
    }
}

function showToast(type, message) {
    let $container = $('#toast-container');
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
    setTimeout(() => closeToast(toastId), 3000);
}

function closeToast(toastId) {
    $(`#${toastId}`).fadeOut(300, function () {
        $(this).remove();
    });
}

// ========================================
// 🛒 SEPET TOGGLE - DÜZELTİLMİŞ ÇÖZÜM
// ========================================

$(document).ready(function () {
    console.log('🔧 Sepet sistemi yükleniyor...');

    // Banner temizle
    $('img[src*="banner-"]').closest('div, section, figure, a').remove();

    // ✅ Bootstrap dropdown'u devre dışı bırak
    $('.cart-toggle').off('click.bs.dropdown');
    $('#shoppingCart').off('show.bs.dropdown hide.bs.dropdown');

    // ✅ Sepet toggle - TEMİZ EVENT BINDING
    $(document).on('click', '.cart-toggle', function (e) {
        e.preventDefault();
        e.stopPropagation();
        e.stopImmediatePropagation(); // ✅ Tüm event propagation'ı durdur

        const $cart = $('.cart-dropdown-custom');
        const $overlay = $('.cart-overlay');
        const isOpen = $cart.hasClass('cart-open');

        console.log('🛒 Sepet tıklandı:', isOpen ? 'Kapatılıyor' : 'Açılıyor');

        if (isOpen) {
            closeCartInternal();
        } else {
            openCartInternal();
        }

        return false; // ✅ Ekstra güvenlik
    });

    // ✅ Internal açma fonksiyonu
    function openCartInternal() {
        $('.cart-dropdown-custom').addClass('cart-open');
        $('.cart-overlay').addClass('active');
        $('body').addClass('cart-open');
        setTimeout(() => refreshCartDropdown(), 100);
        console.log('✅ Sepet açıldı');
    }

    // ✅ Internal kapama fonksiyonu
    function closeCartInternal() {
        $('.cart-dropdown-custom').removeClass('cart-open');
        $('.cart-overlay').removeClass('active');
        $('body').removeClass('cart-open');
        console.log('✅ Sepet kapatıldı');
    }

    // ✅ Overlay'e tıklayınca kapat
    $(document).on('click', '.cart-overlay', function (e) {
        e.stopPropagation();
        closeCartInternal();
    });

    // ✅ Global close fonksiyonu
    window.closeCart = function () {
        closeCartInternal();
    };

    // ✅ ESC tuşu ile kapat
    $(document).on('keydown', function (e) {
        if (e.key === 'Escape' && $('.cart-dropdown-custom').hasClass('cart-open')) {
            closeCartInternal();
        }
    });

    console.log('✅ Sepet hazır!');
});
$(document).ready(function () {
    function updateCartQuantity(productId, quantityInput) {
        let quantity = parseInt(quantityInput.val());
        if (quantity < 1 || isNaN(quantity)) quantity = 1;

        $.ajax({
            url: '/ShoppingCart/UpdateQuantity',
            type: 'POST',
            data: { productId: productId, quantity: quantity },
            success: function (response) {
                if (response.success) {
                    const subtotal = response.updatedSubtotal.toFixed(2);
                    const total = response.totalPrice.toFixed(2);

                    const row = quantityInput.closest('tr');
                    row.find('.subtotal-price').text(subtotal + " ₺");

                    $('.total-price').text(total);
                    $('.total-quantity').text(response.totalQuantity);

                    // ✅ Minicart içindeki ürünü bul ve güncelle
                    const productId = quantityInput.data('product-id');
                    const minicartProduct = $('.dropdown-cart-products .product[data-product-id="' + productId + '"]');

                    if (minicartProduct.length > 0) {
                        minicartProduct.find('.cart-product-qty').text(response.updatedQuantity);
                        minicartProduct.find('.cart-product-info').text(
                            response.updatedQuantity + " × " + subtotal
                        );
                    }

                    // ✅ Minicart toplam fiyatını güncelle
                    $('.cart-total-price').text(total + " ₺");

                    // ✅ Sepet ikonundaki sayı (badge)
                    $('.cart-count').text(response.totalQuantity);

                    // ✅ Yan sepet detay kısmını da güncelle
                    $('.cart-summary-price').text(total + '₺');
                    $('.cart-summary-total').text(total + '₺');
                    $('.cart-summary-quantity').text(response.totalQuantity);
                }
            }
        });
    }

    // input değiştiğinde (manuel ya da butonla)
    $(document).on('change', '.horizontal-quantity', function () {
        const productId = $(this).data('product-id');
        updateCartQuantity(productId, $(this));
    });
});
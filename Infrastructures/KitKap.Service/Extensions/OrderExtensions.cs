using Kitkap.Entity.Entities;
using KitKap.DataAccess.Identity;
using KitKap.Service.Dtos.OrderDtos;
using static Kitkap.Entity.Entities.Order;
using static Kitkap.Entity.Entities.OrderItem;

namespace KitKap.Service.Extensions
{
    public static class OrderExtensions
    {
        // ========================================
        // ENUM → TÜRKÇE TEXT ÇEVİRİLERİ
        // ========================================

        public static string ToDisplayText(this OrderStatus status)
        {
            return status switch
            {
                OrderStatus.Pending => "Ödeme Bekleniyor",
                OrderStatus.PaymentReceived => "Ödeme Alındı",
                OrderStatus.Processing => "Hazırlanıyor",
                OrderStatus.Shipped => "Kargoya Verildi",
                OrderStatus.Delivered => "Teslim Edildi",
                OrderStatus.Cancelled => "İptal Edildi",
                OrderStatus.Refunded => "İade Edildi",
                _ => "Bilinmeyen Durum"
            };
        }

        public static string ToStatusClass(this OrderStatus status)
        {
            return status switch
            {
                OrderStatus.Pending => "badge-warning",
                OrderStatus.PaymentReceived => "badge-info",
                OrderStatus.Processing => "badge-primary",
                OrderStatus.Shipped => "badge-success",
                OrderStatus.Delivered => "badge-success",
                OrderStatus.Cancelled => "badge-danger",
                OrderStatus.Refunded => "badge-secondary",
                _ => "badge-light"
            };
        }

        public static string ToDisplayText(this OrderItemStatus status)
        {
            return status switch
            {
                OrderItemStatus.Pending => "Bekliyor",
                OrderItemStatus.Confirmed => "Onaylandı",
                OrderItemStatus.Shipped => "Gönderildi",
                OrderItemStatus.Delivered => "Teslim Edildi",
                OrderItemStatus.Cancelled => "İptal Edildi",
                _ => "Bilinmeyen"
            };
        }

        public static string ToDisplayText(this string paymentMethod)
        {
            return paymentMethod switch
            {
                "BankTransfer" => "Havale/EFT",
                "CreditCard" => "Kredi Kartı",
                "Cash" => "Kapıda Ödeme",
                _ => "Bilinmeyen Ödeme Yöntemi"
            };
        }

        // ========================================
        // DTO ENRICHMENT
        // ========================================

        /// <summary>
        /// OrderDto'yu zenginleştirir
        /// Service katmanında kullanılır - AppUser ve Address Include edilmiş olmalı
        /// </summary>
        public static void EnrichDto(this OrderDto dto, Order order, AppUser? buyer, Address? address)
        {
            // Enum çevirileri
            dto.StatusText = order.Status.ToDisplayText();
            dto.PaymentMethodText = order.PaymentMethod.ToDisplayText();

            // Buyer bilgileri
            if (buyer != null)
            {
                dto.BuyerName = $"{buyer.FirstName} {buyer.LastName}";
                dto.BuyerEmail = buyer.Email ?? "Email yok";
            }
            else
            {
                dto.BuyerName = "Bilinmeyen Kullanıcı";
                dto.BuyerEmail = "";
            }

            // Adres bilgileri
            if (address != null)
            {
                dto.ShippingAddressText = address.ToDisplayText();
            }
            else
            {
                dto.ShippingAddressText = "Adres bilgisi yok";
            }
        }

        /// <summary>
        /// OrderItemDto'yu zenginleştirir
        /// Service katmanında kullanılır - Product ve Seller Include edilmiş olmalı
        /// </summary>
        public static void EnrichDto(this OrderItemDto dto, OrderItem item, Product? product, AppUser? seller)
        {
            // Enum çevirisi
            dto.StatusText = item.Status.ToDisplayText();

            // Product bilgileri
            if (product != null)
            {
                dto.ProductName = product.Name;
                dto.ProductImageUrl = product.GetMainImageUrl();
            }
            else
            {
                dto.ProductName = "Bilinmeyen Ürün";
                dto.ProductImageUrl = "/images/no-image.png";
            }

            // Seller bilgileri
            if (seller != null)
            {
                dto.SellerName = $"{seller.FirstName} {seller.LastName}";
            }
            else
            {
                dto.SellerName = "Bilinmeyen Satıcı";
            }
        }
    }
}
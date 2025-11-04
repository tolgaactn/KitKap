using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kitkap.Entity.Entities
{
    public class Order
    {
        public int Id { get; set; }

        // ✅ Alıcı (Sipariş veren)
        public string BuyerId { get; set; }

        // ✅ Adres
        public int ShippingAddressId { get; set; }
        public Address ShippingAddress { get; set; }

        // ✅ Sipariş durumu
        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        // ✅ Ödeme yöntemi
        public string PaymentMethod { get; set; }

        // ✅ Sipariş kalemleri
        public ICollection<OrderItem> Items { get; set; }

        // ✅ Transaction
        public Transaction? Transaction { get; set; }

        // ✅ Fiyatlar
        public decimal SubTotal { get; set; }        // Ürün toplamı
        public decimal ShippingCost { get; set; }    // Kargo ücreti
        public decimal CommissionAmount { get; set; } // Komisyon (Marketplace için)
        public decimal TotalAmount { get; set; }     // Genel toplam

        // ✅ Tarihler
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ShippedAt { get; set; }
        public DateTime? DeliveredAt { get; set; }

        // ✅ Kargo
        public string? TrackingNumber { get; set; }
        public string? CargoCompany { get; set; }

        // ✅ Notlar
        public string? CustomerNote { get; set; }
        public string? AdminNote { get; set; }

        public enum OrderStatus
        {
            Pending = 0,
            PaymentReceived = 1,
            Processing = 2,
            Shipped = 3,
            Delivered = 4,
            Cancelled = 5,
            Refunded = 6
        }
    }
}

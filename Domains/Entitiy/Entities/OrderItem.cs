using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kitkap.Entity.Entities
{
    public class OrderItem
    {
        public int Id { get; set; }

        // ✅ Sipariş
        public int OrderId { get; set; }
        public Order Order { get; set; }

        // ✅ Ürün
        public long ProductId { get; set; }
        public Product Product { get; set; }

        // ✅ SATICI BİLGİSİ (Önemli!)
        public string SellerId { get; set; }  // Ürün sahibi

        // ✅ Fiyat bilgileri
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }        // Ürün fiyatı
        public decimal CommissionRate { get; set; }    // Komisyon oranı (0.10 = %10)
        public decimal CommissionAmount { get; set; }  // Komisyon tutarı
        public decimal SellerAmount { get; set; }      // Satıcıya kalacak

        // ✅ Durum
        public OrderItemStatus Status { get; set; } = OrderItemStatus.Pending;

        public enum OrderItemStatus
        {
            Pending = 0,        // Bekliyor
            Confirmed = 1,      // Onaylandı
            Shipped = 2,        // Gönderildi
            Delivered = 3,      // Teslim edildi
            Cancelled = 4       // İptal
        }

    }
}

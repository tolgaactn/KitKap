using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kitkap.Entity.Entities
{
    public class Transaction
    {
        public int Id { get; set; }

        // ✅ Sipariş ile ilişki
        public int OrderId { get; set; }
        public Order Order { get; set; }


        // ✅ Ödeme bilgileri
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; }


        // ✅ Ödeme durumu
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;


        // ✅ Gateway bilgileri (opsiyonel)
        public string? PaymentProvider { get; set; }
        public string? TransactionId { get; set; }
        public string? ErrorMessage { get; set; }


        // ✅ Tarihler
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; }

        public enum PaymentStatus
        {
            Pending = 0,
            Processing = 1,
            Completed = 2,
            Failed = 3,
            Refunded = 4,
            Cancelled = 5
        }
    }
}

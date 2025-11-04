using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Kitkap.Entity.Entities.Transaction;

namespace KitKap.Service.Dtos.TransactionDtos
{
    public class TransactionDto
    {
        public int Id { get; set; }
        public int OrderId { get; set; }

        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentMethodText { get; set; }

        public PaymentStatus Status { get; set; }
        public string StatusText { get; set; }

        public string? PaymentProvider { get; set; }      // "PayTR", null
        public string? TransactionId { get; set; }        // Gateway'den gelen ID
        public string? ErrorMessage { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}

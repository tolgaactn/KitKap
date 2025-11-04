using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kitkap.Service.Dtos.AddressDtos
{
    public class CreateTransactionDto
    {
        public int OrderId { get; set; }
        public string PaymentMethod { get; set; }
        public string? PaymentProvider { get; set; }  // Opsiyonel: "PayTR"
    }
}

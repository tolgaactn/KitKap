using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kitkap.Service.Dtos.AddressDtos
{
    public class UpdateTransactionDto
    {
        public int TransactionId { get; set; }
        public int ProductId { get; set; }
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Status { get; set; }
        public int TrackingCode { get; set; }
        public Decimal PointTransferred { get; set; }
    }
}

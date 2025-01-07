using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kitkap.Entity.ViewModels.TransactionViewModels
{
    public class UpdateTransactionViewModel
    {
        public int TransactionId { get; set; }
        public int BookId { get; set; }
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Status { get; set; }
        public int TrackingCode { get; set; }
        public Decimal PointTransferred { get; set; }
    }
}

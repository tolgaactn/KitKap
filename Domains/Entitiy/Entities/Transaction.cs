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
        public int TransactionId { get; set; }
        public int BookId { get; set; }
        
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Status { get; set; }
        public int TrackingCode { get; set; }
        public Decimal PointTransferred { get; set; }

        public Book Book { get; set; }


    }
}

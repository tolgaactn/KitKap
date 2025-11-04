using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Kitkap.Entity.Entities.OrderItem;

namespace KitKap.Service.Dtos.OrderDtos
{
    public class OrderItemDto
    {
        public int Id { get; set; }

        public long ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductImageUrl { get; set; }

        public string SellerId { get; set; }
        public string SellerName { get; set; }

        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice => UnitPrice * Quantity;

        public decimal CommissionRate { get; set; }
        public decimal CommissionAmount { get; set; }
        public decimal SellerAmount { get; set; }

        public OrderItemStatus Status { get; set; }
        public string StatusText { get; set; }
    }
}

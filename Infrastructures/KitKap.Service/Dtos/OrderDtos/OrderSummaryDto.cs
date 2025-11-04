using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KitKap.Service.Dtos.OrderDtos
{
    public class OrderSummaryDto
    {
        public List<OrderItemDto> Items { get; set; } = new();
        public decimal SubTotal { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal TotalAmount { get; set; }
        public bool IsFreeShipping { get; set; }
        public decimal FreeShippingThreshold { get; set; } = 400;
        public decimal RemainingForFreeShipping { get; set; }
    }
}

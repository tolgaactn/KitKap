using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Kitkap.Entity.Entities.Order;

namespace KitKap.Service.Dtos.OrderDtos
{
    public class OrderDto
    {
        public int Id { get; set; }
        public string BuyerId { get; set; }
        public string BuyerName { get; set; }
        public string BuyerEmail { get; set; }


        public int ShippingaddressId { get; set; }
        public string ShippingAddressText { get; set; }

        public OrderStatus Status { get; set; }
        public string StatusText { get; set; }

        public string PaymentMethod { get; set; }
        public string PaymentMethodText { get; set; }

        public List<OrderItemDto> Items { get; set; } = new();

        public decimal SubTotal { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal TotalAmount { get; set; }

        public DateTime? CreatedAt { get; set; }
        public DateTime? ShippedAt { get; set; }
        public DateTime? DeliveredAt { get; set; }

        public string? TrackingNumber { get; set; }
        public string? CargoCompany { get; set; }

        public string? CustomerNote { get; set; }
        public string? AdminNote { get; set; }
    }
}

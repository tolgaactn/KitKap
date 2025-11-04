using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KitKap.Service.Dtos.OrderDtos
{
    public class CreateOrderDto
    {
        public string BuyerId { get; set; }
        public int ShippingAddressId { get; set; }
        public string PaymentMethod { get; set; }
        public string? CustomerNote { get; set; }
    }
}

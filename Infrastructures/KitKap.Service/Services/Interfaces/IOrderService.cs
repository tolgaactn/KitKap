using KitKap.Service.Dtos.OrderDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Kitkap.Entity.Entities.Order;

namespace KitKap.Service.Services.Interfaces
{
    public interface IOrderService
    {
        Task<OrderSummaryDto> GetOrderSummaryAsync(string userId);

        Task<int> CreateOrderFromCartAsync(CreateOrderDto dto);

        Task<IEnumerable<OrderDto>> GetOrdersByUserAsync(string userId);

        Task UpdateOrderStatusAsync(int orderId, OrderStatus newStatus);

        Task UpdateShippingInfoAsync(int orderId, string trackingNumber, string cargoCompany);
        Task<OrderDto> GetOrderByIdAsync(int orderId);
    }
}

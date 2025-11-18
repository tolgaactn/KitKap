using KitKap.DataAccess.Identity;
using KitKap.MvcUI.ViewModels.OrderViewModels;
using KitKap.Service.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static Kitkap.Entity.Entities.Order;

namespace KitKap.MvcUI.Areas.Admin.Controllers
{
    [Route("Admin/Order")]
    public class OrderController : BaseAdminController
    {
        private readonly IOrderService _orderService;
        private readonly UserManager<AppUser> _userManager;

        public OrderController(
            IOrderService orderService,
            UserManager<AppUser> userManager)
        {
            _orderService = orderService;
            _userManager = userManager;
        }

        [Route("")]
        [Route("Index")]
        public async Task<IActionResult> Index(string? status, string? search)
        {
            var allOrders = new List<OrderListViewModel>();

            // Tüm kullanıcıların siparişlerini topla
            var allUsers = _userManager.Users.ToList();

            foreach (var user in allUsers)
            {
                var userOrders = await _orderService.GetOrdersByUserAsync(user.Id);

                var orderViewModels = userOrders.Select(o => new OrderListViewModel
                {
                    Id = o.Id,
                    OrderNumber = $"ORD-{o.Id:D6}",
                    CustomerName = $"{user.FirstName} {user.LastName}",
                    CustomerEmail = user.Email,
                    TotalAmount = o.TotalAmount,
                    Status = o.Status.ToString(),
                    StatusText = o.StatusText ?? GetStatusText(o.Status),
                    OrderDate = o.CreatedAt ?? DateTime.Now,
                    ItemCount = o.Items?.Count ?? 0
                });

                allOrders.AddRange(orderViewModels);
            }

            // Filtreleme - Status
            if (!string.IsNullOrEmpty(status) && status != "All")
            {
                allOrders = allOrders.Where(o => o.Status == status).ToList();
            }

            // Filtreleme - Search
            if (!string.IsNullOrEmpty(search))
            {
                allOrders = allOrders.Where(o =>
                    o.CustomerName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    o.CustomerEmail.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    o.OrderNumber.Contains(search, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }

            // Tarihe göre sırala (en yeni üstte)
            allOrders = allOrders.OrderByDescending(o => o.OrderDate).ToList();

            ViewData["TotalCount"] = allOrders.Count;
            ViewData["SelectedStatus"] = status ?? "All";
            ViewData["SearchQuery"] = search;

            return View(allOrders);
        }

        [Route("Detail/{id}")]
        public async Task<IActionResult> Detail(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);

            if (order == null)
            {
                SetErrorMessage("Sipariş bulunamadı!");
                return RedirectToAction("Index");
            }

            var user = await _userManager.FindByIdAsync(order.BuyerId);

            var viewModel = new OrderDetailViewModel
            {
                Id = order.Id,
                OrderNumber = $"ORD-{order.Id:D6}",
                BuyerName = order.BuyerName,
                BuyerEmail = order.BuyerEmail,
                BuyerPhone = user?.PhoneNumber ?? "Belirtilmemiş",
                ShippingAddress = order.ShippingAddressText,
                Status = order.Status.ToString(),
                StatusText = order.StatusText ?? GetStatusText(order.Status),
                PaymentMethod = order.PaymentMethodText ?? order.PaymentMethod,
                Items = order.Items.Select(item => new OrderItemViewModel
                {
                    ProductName = item.ProductName,
                    ProductImage = item.ProductImageUrl ?? "/template/assets/images/default-product.png",
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    TotalPrice = item.Quantity * item.UnitPrice
                }).ToList(),
                SubTotal = order.SubTotal,
                ShippingCost = order.ShippingCost,
                TotalAmount = order.TotalAmount,
                OrderDate = order.CreatedAt ?? DateTime.Now,
                ShippedAt = order.ShippedAt,
                DeliveredAt = order.DeliveredAt,
                TrackingNumber = order.TrackingNumber,
                CargoCompany = order.CargoCompany,
                CustomerNote = order.CustomerNote,
                AdminNote = order.AdminNote
            };

            return View(viewModel);
        }

        [HttpPost]
        [Route("UpdateStatus")]
        public async Task<IActionResult> UpdateStatus(UpdateOrderStatusViewModel model)
        {
            try
            {
                if (!Enum.TryParse<OrderStatus>(model.NewStatus, out var newStatus))
                {
                    SetErrorMessage("Geçersiz sipariş durumu!");
                    return RedirectToAction("Detail", new { id = model.OrderId });
                }

                await _orderService.UpdateOrderStatusAsync(model.OrderId, newStatus);

                // Eğer kargo bilgisi varsa güncelle
                if (!string.IsNullOrEmpty(model.TrackingNumber) || !string.IsNullOrEmpty(model.CargoCompany))
                {
                    await _orderService.UpdateShippingInfoAsync(
                        model.OrderId,
                        model.TrackingNumber ?? "",
                        model.CargoCompany ?? ""
                    );
                }

                SetSuccessMessage("Sipariş durumu başarıyla güncellendi!");
                return RedirectToAction("Detail", new { id = model.OrderId });
            }
            catch (Exception ex)
            {
                SetErrorMessage($"Hata: {ex.Message}");
                return RedirectToAction("Detail", new { id = model.OrderId });
            }
        }

        [HttpPost]
        [Route("UpdateShipping")]
        public async Task<IActionResult> UpdateShipping(int orderId, string trackingNumber, string cargoCompany)
        {
            try
            {
                await _orderService.UpdateShippingInfoAsync(orderId, trackingNumber, cargoCompany);
                SetSuccessMessage("Kargo bilgileri başarıyla güncellendi!");
                return RedirectToAction("Detail", new { id = orderId });
            }
            catch (Exception ex)
            {
                SetErrorMessage($"Hata: {ex.Message}");
                return RedirectToAction("Detail", new { id = orderId });
            }
        }

        // Helper method - Status text dönüşümü
        private string GetStatusText(OrderStatus status)
        {
            return status switch
            {
                OrderStatus.Pending => "Beklemede",
                OrderStatus.PaymentReceived => "Ödeme Alındı",
                OrderStatus.Processing => "Hazırlanıyor",
                OrderStatus.Shipped => "Kargoya Verildi",
                OrderStatus.Delivered => "Teslim Edildi",
                OrderStatus.Cancelled => "İptal Edildi",
                OrderStatus.Refunded => "İade Edildi",
                _ => status.ToString()
            };
        }
    }
}
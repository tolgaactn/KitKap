using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using KitKap.Service.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using KitKap.DataAccess.Identity;

namespace KitKap.MvcUI.Controllers
{
    [Authorize] // Sadece giriş yapmış kullanıcılar erişebilir
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly UserManager<AppUser> _userManager;

        public OrderController(IOrderService orderService, UserManager<AppUser> userManager)
        {
            _orderService = orderService;
            _userManager = userManager;
        }

        // ========================================
        // SİPARİŞLERİM SAYFASI
        // ========================================
        [HttpGet]
        public async Task<IActionResult> MyOrders()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("MyOrders", "Order") });
                }

                var orders = await _orderService.GetOrdersByUserAsync(user.Id);
                return View(orders);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Siparişler yüklenirken bir hata oluştu: " + ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        // ========================================
        // SİPARİŞ DETAY MODAL (AJAX)
        // ========================================
        [HttpGet]
        public async Task<IActionResult> GetOrderDetail(int id)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Json(new { success = false, message = "Kullanıcı bulunamadı." });
                }

                var order = await _orderService.GetOrderByIdAsync(id);

                // Güvenlik: Kullanıcı sadece kendi siparişini görebilir
                if (order.BuyerId != user.Id)
                {
                    return Json(new { success = false, message = "Bu siparişe erişim yetkiniz yok." });
                }

                return PartialView("_OrderDetailModal", order);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Sipariş detayı yüklenirken hata oluştu: " + ex.Message });
            }
        }
    }
}
using Kitkap.Entity.Services;
using Kitkap.Service.Dtos.AddressDtos;
using KitKap.MvcUI.ViewModels.CheckoutViewModels;
using KitKap.Service.Dtos.OrderDtos;
using KitKap.Service.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace KitKap.MvcUI.Controllers
{
    [Authorize] // ✅ Sadece giriş yapmış kullanıcılar
    public class CheckoutController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IAddressService _addressService;
        private readonly ITransactionService _transactionService;
        private readonly IShoppingCartService _shoppingCartService;

        public CheckoutController(
            IOrderService orderService,
            IAddressService addressService,
            ITransactionService transactionService,
            IShoppingCartService shoppingCartService)
        {
            _orderService = orderService;
            _addressService = addressService;
            _transactionService = transactionService;
            _shoppingCartService = shoppingCartService;
        }

        // ========================================
        // 1. CHECKOUT SAYFASI (GET)
        // ========================================

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                // 1. Sipariş özetini al
                var orderSummary = await _orderService.GetOrderSummaryAsync(userId);

                // 2. Kullanıcının adreslerini al
                var addresses = await _addressService.GetByUserIdAsync(userId);

                // 3. ViewModel oluştur
                var viewModel = new CheckoutViewModel
                {
                    OrderSummary = orderSummary,
                    UserAddresses = addresses.ToList(),
                    UserId = userId,
                    UserEmail = User.FindFirstValue(ClaimTypes.Email) ?? "",
                    UserFullName = User.FindFirstValue(ClaimTypes.Name) ?? "",
                    PaymentMethod = "BankTransfer" // Varsayılan
                };

                // Varsayılan adresi seç (ilk adres)
                if (viewModel.UserAddresses.Any())
                {
                    viewModel.SelectedAddressId = viewModel.UserAddresses.First().Id;
                }

                return View(viewModel);
            }
            catch (InvalidOperationException ex)
            {
                // Sepet boş
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index", "ShoppingCart");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Bir hata oluştu: {ex.Message}";
                return RedirectToAction("Index", "Home");
            }
        }

        // ========================================
        // 2. SİPARİŞİ TAMAMLA (POST)
        // ========================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder(CheckoutViewModel model)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                // Model validasyon
                if (model.SelectedAddressId == 0)
                {
                    TempData["ErrorMessage"] = "Lütfen bir teslimat adresi seçin";
                    return RedirectToAction("Index");
                }

                if (string.IsNullOrEmpty(model.PaymentMethod))
                {
                    TempData["ErrorMessage"] = "Lütfen bir ödeme yöntemi seçin";
                    return RedirectToAction("Index");
                }

                // CreateOrderDto oluştur
                var createOrderDto = new CreateOrderDto
                {
                    BuyerId = userId,
                    ShippingAddressId = model.SelectedAddressId,
                    PaymentMethod = model.PaymentMethod,
                    CustomerNote = model.CustomerNote
                };

                // ✅ DEBUG: Console'a yaz
                Console.WriteLine($"🔵 CreateOrder başlıyor...");
                Console.WriteLine($"   - BuyerId: {userId}");
                Console.WriteLine($"   - ShippingAddressId: {model.SelectedAddressId}");
                Console.WriteLine($"   - PaymentMethod: {model.PaymentMethod}");

                // Siparişi oluştur
                var orderId = await _orderService.CreateOrderFromCartAsync(createOrderDto);

                Console.WriteLine($"✅ Order oluşturuldu - OrderId: {orderId}");

                // Onay sayfasına yönlendir
                return RedirectToAction("Confirmation", new { orderId = orderId });
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"⚠️ InvalidOperationException: {ex.Message}");
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // ✅ INNER EXCEPTION'I GÖSTER
                Console.WriteLine($"❌ HATA:");
                Console.WriteLine($"   Message: {ex.Message}");
                Console.WriteLine($"   InnerException: {ex.InnerException?.Message}");
                Console.WriteLine($"   StackTrace: {ex.StackTrace}");

                TempData["ErrorMessage"] = $"Sipariş oluşturulurken hata: {ex.InnerException?.Message ?? ex.Message}";
                return RedirectToAction("Index");
            }
        }

        // ========================================
        // 3. SİPARİŞ ONAY SAYFASI (GET)
        // ========================================

        [HttpGet]
        public async Task<IActionResult> Confirmation(int orderId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                // 1. Siparişi getir
                var order = await _orderService.GetOrderByIdAsync(orderId);

                // 2. Sipariş kullanıcıya ait mi kontrol et
                if (order.BuyerId != userId)
                {
                    TempData["ErrorMessage"] = "Bu siparişe erişim yetkiniz yok";
                    return RedirectToAction("Index", "Home");
                }

                // 3. Transaction'ı getir
                var transaction = await _transactionService.GetTransactionByOrderIdAsync(orderId);

                // 4. ViewModel oluştur
                var viewModel = new OrderConfirmationViewModel
                {
                    Order = order,
                    Transaction = transaction
                };

                return View(viewModel);
            }
            catch (KeyNotFoundException)
            {
                TempData["ErrorMessage"] = "Sipariş bulunamadı";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Bir hata oluştu: {ex.Message}";
                return RedirectToAction("Index", "Home");
            }
        }

        // ========================================
        // 4. YENİ ADRES EKLE (AJAX)
        // ========================================

        [HttpPost]
        public async Task<IActionResult> AddNewAddress([FromBody] CreateAddressDto dto)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                dto.UserId = userId;

                await _addressService.AddAsync(dto);

                return Json(new { success = true, message = "Adres başarıyla eklendi" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // ========================================
        // 5. SİPARİŞ ÖZETİNİ YENİLE (AJAX)
        // ========================================

        [HttpGet]
        public async Task<IActionResult> RefreshOrderSummary()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var orderSummary = await _orderService.GetOrderSummaryAsync(userId);

                return Json(new
                {
                    success = true,
                    subTotal = orderSummary.SubTotal,
                    shippingCost = orderSummary.ShippingCost,
                    totalAmount = orderSummary.TotalAmount,
                    isFreeShipping = orderSummary.IsFreeShipping,
                    remainingForFreeShipping = orderSummary.RemainingForFreeShipping
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

    }
}
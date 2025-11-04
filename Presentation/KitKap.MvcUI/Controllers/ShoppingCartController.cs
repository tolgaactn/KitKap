using KitKap.MvcUI.ViewModels.ShoppingCartViewModels;
using KitKap.Service.Dtos.ShoppingCartDtos;
using KitKap.Service.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace KitKap.MvcUI.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly IShoppingCartService _shoppingCartService;

        public ShoppingCartController(IShoppingCartService shoppingCartService)
        {
            _shoppingCartService = shoppingCartService;
        }

        // ✅ AJAX ile sepete ekleme
        [HttpPost]
        public async Task<IActionResult> AddToCart(long productId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var guestId = CookieHelper.GetOrCreateGuestId(HttpContext);

                var dto = new AddCartItemDto
                {
                    ProductId = productId,
                    Quantity = 1
                };

                await _shoppingCartService.AddToCartAsync(userId, guestId, dto);

                // Güncel sepet bilgisini al
                var cart = await _shoppingCartService.GetCartAsync(userId, guestId);

                return Json(new
                {
                    success = true,
                    message = "Ürün sepete eklendi!",
                    cartItemCount = cart.TotalItemCount,
                    cartTotal = cart.TotalPrice.ToString("N2")
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = $"Hata: {ex.Message}"
                });
            }
        }

        // ✅ Dropdown sepeti güncelleme (Partial View döndür)
        [HttpGet]
        public async Task<IActionResult> GetCartDropdown()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var guestId = CookieHelper.GetOrCreateGuestId(HttpContext);

            var cartDto = await _shoppingCartService.GetCartAsync(userId, guestId);

            var model = new ShoppingCartViewModel
            {
                Id = cartDto.Id,
                UserId = cartDto.UserId,
                GuestId = cartDto.GuestId,
                CreatedAt = cartDto.CreatedAt,
                Items = cartDto.Items.Select(item => new ShoppingCartItemViewModel
                {
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    ImageUrl = item.ProductImageUrl
                }).ToList()
            };

            return PartialView("Components/_CartDropdownComponentPartial/Default", model);
        }

        // ✅ Dropdown'dan ürün silme
        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(long productId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var guestId = CookieHelper.GetOrCreateGuestId(HttpContext);

                await _shoppingCartService.RemoveFromCartAsync(userId, guestId, productId);

                var cart = await _shoppingCartService.GetCartAsync(userId, guestId);

                return Json(new
                {
                    success = true,
                    message = "Ürün sepetten kaldırıldı",
                    cartItemCount = cart.TotalItemCount,
                    cartTotal = cart.TotalPrice.ToString("N2")
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = $"Hata: {ex.Message}"
                });
            }
        }

        // Sepet sayfası
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var guestId = CookieHelper.GetOrCreateGuestId(HttpContext);

            var cartDto = await _shoppingCartService.GetCartAsync(userId, guestId);

            var model = new ShoppingCartViewModel
            {
                Id = cartDto.Id,
                UserId = cartDto.UserId,
                GuestId = cartDto.GuestId,
                CreatedAt = cartDto.CreatedAt,
                Items = cartDto.Items.Select(item => new ShoppingCartItemViewModel
                {
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    ImageUrl = item.ProductImageUrl
                }).ToList()
            };

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(long productId, int quantity)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var guestId = CookieHelper.GetOrCreateGuestId(HttpContext);

                await _shoppingCartService.UpdateQuantityAsync(userId, guestId, productId, quantity);

                var cart = await _shoppingCartService.GetCartAsync(userId, guestId);

                return Json(new
                {
                    success = true,
                    cartItemCount = cart.TotalItemCount,
                    cartTotal = cart.TotalPrice.ToString("N2")
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }
        [HttpPost]
        public async Task<IActionResult> ClearCart()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var guestId = CookieHelper.GetOrCreateGuestId(HttpContext);

                await _shoppingCartService.ClearCartAsync(userId, guestId);

                return Json(new
                {
                    success = true,
                    message = "Sepet başarıyla temizlendi!"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Sepet temizlenirken hata oluştu: " + ex.Message
                });
            }
        }
    }
}
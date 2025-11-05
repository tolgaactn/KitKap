using KitKap.MvcUI.ViewModels.ShoppingCartViewModels;
using KitKap.Service.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace KitKap.MvcUI.ViewComponents.ShoppingCartViewComponents
{
    public class _ShoppingCartDetailComponentPartial : ViewComponent
    {
        private readonly IShoppingCartService _shoppingCartService;

        public _ShoppingCartDetailComponentPartial(IShoppingCartService shoppingCartService)
        {
            _shoppingCartService = shoppingCartService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
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
    }
}
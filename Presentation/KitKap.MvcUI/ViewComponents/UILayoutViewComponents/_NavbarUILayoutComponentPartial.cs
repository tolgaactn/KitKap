using KitKap.DataAccess.Identity;
using KitKap.MvcUI.ViewModels.NavbarViewModels;
using KitKap.MvcUI.ViewModels.ShoppingCartViewModels;
using KitKap.Service.Extensions;
using KitKap.Service.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace KitKap.MvcUI.ViewComponents.UILayoutViewComponents
{
    public class _NavbarUILayoutComponentPartial : ViewComponent
    {
        private readonly IShoppingCartService _shoppingCartService;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;

        public _NavbarUILayoutComponentPartial(
            IShoppingCartService shoppingCartService,
            SignInManager<AppUser> signInManager,
            UserManager<AppUser> userManager)
        {
            _shoppingCartService = shoppingCartService;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var guestId = CookieHelper.GetOrCreateGuestId(HttpContext);

            // Sepet bilgisi
            var cartDto = await _shoppingCartService.GetCartAsync(userId, guestId);

            // Kullanıcı bilgisi
            bool isSignedIn = _signInManager.IsSignedIn(HttpContext.User);
            AppUser? currentUser = null;

            if (isSignedIn && userId != null)
            {
                currentUser = await _userManager.FindByIdAsync(userId);
            }

            var model = new NavbarViewModel
            {
                Cart = new ShoppingCartViewModel
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
                },

                IsSignedIn = isSignedIn,
                UserFirstName = currentUser?.FirstName ?? "Kullanıcı",
                UserFullName = currentUser != null
                    ? $"{currentUser.FirstName} {currentUser.LastName}".Trim()
                    : null
            };

            return View(model);
        }
    }
}
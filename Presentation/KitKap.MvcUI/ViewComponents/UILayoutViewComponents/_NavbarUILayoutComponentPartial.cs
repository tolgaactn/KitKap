using KitKap.MvcUI.ViewModels.ShoppingCartDetailViewModels;
using KitKap.Service.Dtos.ShoppingCartDetailDtos;
using KitKap.Service.Extensions;
using KitKap.Service.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace KitKap.MvcUI.ViewComponents.UILayoutViewComponents
{
    public class _NavbarUILayoutComponentPartial : ViewComponent
    {
        private readonly IProductImageService _productImageService;

        public _NavbarUILayoutComponentPartial(IProductImageService productImageService)
        {
            _productImageService = productImageService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var shoppingCartDto = GetShoppingCart();

            var shoppingCartViewModel = new List<ShoppingCartDetailViewModel>();

            foreach (var dto in shoppingCartDto)
            {
                var productImages = await _productImageService.GetByIdProductImagesAsync(dto.productId);
                var imageUrls = productImages.Where(img => img.IsMain).Select(img => img.ImageUrl).ToList();

                shoppingCartViewModel.Add(new ShoppingCartDetailViewModel
                {
                    productId = dto.productId,
                    productName = dto.productName,
                    productQuantity = dto.productQuantity,
                    productPrice = dto.productPrice,
                    ImageUrls = imageUrls
                });
            }
            return View(shoppingCartViewModel);
        }
        public List<ResultShoppingCartDetailDto> GetShoppingCart()
        {
            var shoppingCart = HttpContext.Session.GetJson<List<ResultShoppingCartDetailDto>>("shoppingCart") ?? new List<ResultShoppingCartDetailDto>();

            return shoppingCart;
        }
    }
}

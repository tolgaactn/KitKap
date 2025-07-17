using AutoMapper;
using Kitkap.Service.Services;
using KitKap.MvcUI.ViewModels.ProductListViewModels;
using KitKap.Service.Dtos.ProductDtos;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace KitKap.MvcUI.ViewComponents.ProductDetailViewComponents
{
    public class _ProductDetailRelatedProductsComponentPartial : ViewComponent
    {
        private readonly IProductService _productService;

        public _ProductDetailRelatedProductsComponentPartial(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var productDtos = await _productService.GetAllProducts();

            var viewModels = productDtos.Select(dto => new ProductListViewModel
            {
                Id = dto.Id,
                Name = dto.Name,
                Price = dto.Price,
                ImageUrl = dto.ProductImages.FirstOrDefault(img => img.IsMain && !img.IsDeleted)?.ImageUrl ?? "/template/assets/images/default-product.png",
                CategoryName = dto.CategoryName
            }).ToList();

            return View(viewModels);
        }
    }
}

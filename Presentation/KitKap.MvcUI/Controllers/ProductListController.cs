using AutoMapper;
using Kitkap.Entity.Services;
using Kitkap.Service.Services;
using KitKap.MvcUI.Areas.Admin.ViewModels.ProductViewModels;
using KitKap.MvcUI.ViewModels.ProductDetailViewModels;
using KitKap.MvcUI.ViewModels.ProductListViewModels;
using KitKap.Service.Dtos.ProductDtos;
using Microsoft.AspNetCore.Mvc;

namespace KitKap.MvcUI.Controllers
{
    public class ProductListController : Controller
    {
        public readonly IProductService _productService;
        public readonly ICategoryService _categoryService;
        public readonly IAccountService _accountService;

        public ProductListController(IProductService productService, ICategoryService categoryService, IAccountService accountService)
        {
            _productService = productService;
            _categoryService = categoryService;
            _accountService = accountService;
        }
        public async Task<IActionResult> Index()
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
        public async Task<IActionResult> ProductDetail(long id)
        {
            var productDto = await _productService.GetByIdProduct(id);

            var owner = await _accountService.FindById(productDto.OwnerId);

            var viewModel = new ProductDetailViewModel()
            {
                Id = productDto.Id,
                Name = productDto.Name,
                Description = productDto.Description,
                Price = productDto.Price,
                Stock = productDto.Stock,
                CategoryName= productDto.CategoryName,
                ImageUrl = productDto.ProductImages.FirstOrDefault(img => img.IsMain && !img.IsDeleted)?.ImageUrl ?? "/template/assets/images/default-product.png",
                OwnerUserName = owner.FirstName
            };
            return View(viewModel);
        }
    }
}

using AutoMapper;
using Kitkap.Entity.Entities;
using Kitkap.Entity.Services;
using Kitkap.Service.Dtos.AddressDtos;
using Kitkap.Service.Services;
using KitKap.MvcUI.Areas.Admin.ViewModels.AboutViewModels;
using KitKap.MvcUI.Areas.Admin.ViewModels.ProductViewModels;
using KitKap.MvcUI.ViewModels.ProductDetailViewModels;
using KitKap.MvcUI.ViewModels.ProductListViewModels;
using KitKap.Service.Dtos.ProductDtos;
using KitKap.Service.Dtos.ShoppingCartDetailDtos;
using KitKap.Service.Extensions;
using KitKap.Service.Services.Concretes;
using KitKap.Service.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace KitKap.MvcUI.Controllers
{
    public class ProductListController : Controller
    {
        public readonly IProductService _productService;
        public readonly ICategoryService _categoryService;
        public readonly IAccountService _accountService;
        private readonly IAboutService _aboutService;
        private readonly IProductImageService _productImageService;
        private readonly IShoppingCartDetailService _shoppingCartDetailService;

        public ProductListController(IProductService productService, ICategoryService categoryService, IAccountService accountService, IAboutService aboutService, IProductImageService productImageService, IShoppingCartDetailService shoppingCartDetailService)
        {
            _productService = productService;
            _categoryService = categoryService;
            _accountService = accountService;
            _aboutService = aboutService;
            _productImageService = productImageService;
            _shoppingCartDetailService = shoppingCartDetailService;
        }
        public async Task<IActionResult> Index()
        {

            var shoppingCart = HttpContext.Session.GetJson<List<ResultShoppingCartDetailDto>>("shoppingCart") ?? new List<ResultShoppingCartDetailDto>();
            TempData["TotalQuantity"] = _shoppingCartDetailService.TotalQuantity(shoppingCart);
            TempData["TotalPrice"] = _shoppingCartDetailService.TotalPrice(shoppingCart);

            var productDtos = await _productService.GetAllProducts();

            var viewModels = productDtos.Select(dto => new ProductListViewModel
            {
                Id = dto.Id,
                Name = dto.Name,
                Price = dto.Price,
                ImageUrl = dto.ProductImages.FirstOrDefault(img => img.IsMain && !img.IsDeleted)?.ImageUrl ?? "/template/assets/images/default-product.png",
                CategoryName = dto.CategoryName
            }).ToList();

            var aboutDtos = await _aboutService.GetAllAboutAsync();

            var viewModel = aboutDtos.Select(aboutDto => new AboutViewModel
            {
                AboutId = aboutDto.AboutId,
                Description = aboutDto.Description,
                Address = aboutDto.Address,
                Email = aboutDto.Email,
                Phone = aboutDto.Phone
            }).ToList();

            ViewBag.About = viewModel;

            return View(viewModels);
        }
        public async Task<IActionResult> ProductDetail(long id)
        {

            var shoppingCart = HttpContext.Session.GetJson<List<ResultShoppingCartDetailDto>>("shoppingCart") ?? new List<ResultShoppingCartDetailDto>();
            TempData["TotalQuantity"] = _shoppingCartDetailService.TotalQuantity(shoppingCart);
            TempData["TotalPrice"] = _shoppingCartDetailService.TotalPrice(shoppingCart);

            var productDto = await _productService.GetByIdProduct(id);

            var productImages = await _productImageService.GetByIdProductImagesAsync(id);

            var owner = await _accountService.FindById(productDto.OwnerId);

            // Tüm ürünleri sıraya al (ister ID'ye, ister başka bir kritere göre)
            var allProducts = (await _productService.GetAllProducts()).Where(p => !p.IsDeleted).OrderBy(p => p.Id).ToList();

            var currentIndex = allProducts.FindIndex(p => p.Id == id);

            var previousProduct = currentIndex > 0 ? allProducts[currentIndex - 1] : null;
            var nextProduct = currentIndex < allProducts.Count - 1 ? allProducts[currentIndex + 1] : null;

            var viewModel = new ProductDetailViewModel()
            {
                Id = productDto.Id,
                Name = productDto.Name,
                Description = productDto.Description,
                Price = productDto.Price,
                Stock = productDto.Stock,
                CategoryName= productDto.CategoryName,
                ImageUrl = productDto.ProductImages.FirstOrDefault(img => img.IsMain && !img.IsDeleted)?.ImageUrl ?? "/template/assets/images/default-product.png",
                ImageUrls = productImages.Where(img => !img.IsDeleted).Select(img => img.ImageUrl).ToList(),
                OwnerUserName = owner.FirstName,

                Author = productDto.Author,
                ISBN = productDto.ISBN,
                PublicationDate = productDto.PublicationDate,
                Language = productDto.Language,
                Condition = productDto.Condition,

                PreviousProduct = previousProduct != null
                    ? new ProductPreviewViewModel
                    {
                        Id = previousProduct.Id,
                        Name = previousProduct.Name,
                        ThumbnailUrl = previousProduct.ProductImages?.FirstOrDefault(img => img.IsMain && !img.IsDeleted)?.ImageUrl
                               ?? "/template/assets/images/default-product.png"
                    }
                    : null,
                NextProduct = nextProduct != null
                    ? new ProductPreviewViewModel
                    {
                        Id = nextProduct.Id,
                        Name = nextProduct.Name,
                        ThumbnailUrl = nextProduct.ProductImages?.FirstOrDefault(img => img.IsMain && !img.IsDeleted)?.ImageUrl
                               ?? "/template/assets/images/default-product.png"
                    }
                    : null,
                IsLastProduct = currentIndex == allProducts.Count - 1
            };

            var aboutDtos = await _aboutService.GetAllAboutAsync();

            var viewModels = aboutDtos.Select(aboutDto => new AboutViewModel
            {
                AboutId = aboutDto.AboutId,
                Description = aboutDto.Description,
                Address = aboutDto.Address,
                Email = aboutDto.Email,
                Phone = aboutDto.Phone
            }).ToList();

            ViewBag.About = viewModels;

            return View(viewModel);
        }
    }
}

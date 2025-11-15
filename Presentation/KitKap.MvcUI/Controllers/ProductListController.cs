using Kitkap.Entity.Services;
using Kitkap.Service.Dtos.CategoryDtos;
using Kitkap.Service.Services;
using KitKap.MvcUI.Areas.Admin.ViewModels.AboutViewModels;
using KitKap.MvcUI.ViewModels;
using KitKap.MvcUI.ViewModels.ProductDetailViewModels;
using KitKap.MvcUI.ViewModels.ProductListViewModels;
using KitKap.Service.Dtos.ProductDtos;
using KitKap.Service.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KitKap.MvcUI.Controllers
{
    public class ProductListController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IAboutService _aboutService;
        private readonly IAccountService _accountService;
        private readonly IProductImageService _productImageService;

        public ProductListController(
            IProductService productService,
            ICategoryService categoryService,
            IAboutService aboutService,
            IAccountService accountService,
            IProductImageService productImageService)
        {
            _productService = productService;
            _categoryService = categoryService;
            _aboutService = aboutService;
            _accountService = accountService;
            _productImageService = productImageService;
        }

        public async Task<IActionResult> Index(ProductFilterViewModel filters)
        {
            // 1. ViewModel'den Service DTO'ya çevir
            var filterDto = new ProductFilterDto
            {
                CategoryId = filters.CategoryId,
                MinPrice = filters.MinPrice,
                MaxPrice = filters.MaxPrice,
                Condition = filters.Condition,
                InStockOnly = filters.InStockOnly,
                SearchQuery = filters.SearchQuery,
                SortBy = filters.SortBy ?? "newest",
                PageNumber = filters.PageNumber,
                PageSize = filters.PageSize
            };

            // 2. Service'den filtrelenmiş ürünleri al (FİLTRELEME SERVİS'TE YAPILIYOR)
            var filterResult = await _productService.GetFilteredProductsAsync(filterDto);

            // 3. Ürünleri MVC ViewModel'e dönüştür
            var productViewModels = filterResult.Products.Select(dto => new ProductListViewModel
            {
                Id = dto.Id,
                Name = dto.Name,
                Price = dto.Price,
                ImageUrl = dto.ProductImages?.FirstOrDefault(img => img.IsMain && !img.IsDeleted)?.ImageUrl
                    ?? "/template/assets/images/default-product.png",
                CategoryName = dto.CategoryName
            }).ToList();

            // 4. Kategorileri çek (Filtre için)
            var categories = await _categoryService.GetAllCategories();
            var categoryViewModels = categories.Select(c => new CategoryViewModel
            {
                Id = c.Id,
                Name = c.Name,
                ProductCount = 0 // TODO: Service'den kategori bazlı count çek
            }).ToList();

            // 5. Aktif filtreleri oluştur
            var activeFilters = BuildActiveFilters(filters, categories.ToList());

            // 6. Ana ViewModel
            var viewModel = new ProductListPageViewModel
            {
                Products = productViewModels,
                Filters = filters,
                Categories = categoryViewModels,
                TotalProducts = filterResult.TotalProducts,
                TotalPages = filterResult.TotalPages,
                CurrentPage = filterResult.CurrentPage,
                MinAvailablePrice = filterResult.MinAvailablePrice,
                MaxAvailablePrice = filterResult.MaxAvailablePrice,
                ActiveFilters = activeFilters
            };

            // 7. About bilgileri (Footer için)
            var aboutDtos = await _aboutService.GetAllAboutAsync();
            ViewBag.About = aboutDtos.Select(dto => new AboutViewModel
            {
                AboutId = dto.AboutId,
                Description = dto.Description,
                Address = dto.Address,
                Email = dto.Email,
                Phone = dto.Phone
            }).ToList();

            return View(viewModel);
        }

        // ✅ HELPER: Aktif filtreleri oluştur (Sadece UI için)
        private List<ActiveFilterViewModel> BuildActiveFilters(
            ProductFilterViewModel filters,
            List<ResultCategoryDto> categories)
        {
            var activeFilters = new List<ActiveFilterViewModel>();

            // Kategori
            if (filters.CategoryId.HasValue && filters.CategoryId.Value > 0)
            {
                var category = categories.FirstOrDefault(c => c.Id == filters.CategoryId.Value);
                if (category != null)
                {
                    activeFilters.Add(new ActiveFilterViewModel
                    {
                        Label = "Kategori",
                        Value = category.Name,
                        RemoveUrl = Url.Action("Index", new
                        {
                            CategoryId = (int?)null,
                            filters.MinPrice,
                            filters.MaxPrice,
                            filters.Condition,
                            filters.InStockOnly,
                            filters.SortBy,
                            filters.SearchQuery,
                            PageNumber = 1
                        })
                    });
                }
            }

            // Fiyat
            if (filters.MinPrice.HasValue || filters.MaxPrice.HasValue)
            {
                activeFilters.Add(new ActiveFilterViewModel
                {
                    Label = "Fiyat",
                    Value = $"{filters.MinPrice ?? 0:N0}₺ - {filters.MaxPrice ?? 999999:N0}₺",
                    RemoveUrl = Url.Action("Index", new
                    {
                        filters.CategoryId,
                        MinPrice = (decimal?)null,
                        MaxPrice = (decimal?)null,
                        filters.Condition,
                        filters.InStockOnly,
                        filters.SortBy,
                        filters.SearchQuery,
                        PageNumber = 1
                    })
                });
            }

            // Durum
            if (!string.IsNullOrEmpty(filters.Condition))
            {
                activeFilters.Add(new ActiveFilterViewModel
                {
                    Label = "Durum",
                    Value = filters.Condition == "New" ? "Sıfır Ürün" : "İkinci El",
                    RemoveUrl = Url.Action("Index", new
                    {
                        filters.CategoryId,
                        filters.MinPrice,
                        filters.MaxPrice,
                        Condition = (string?)null,
                        filters.InStockOnly,
                        filters.SortBy,
                        filters.SearchQuery,
                        PageNumber = 1
                    })
                });
            }

            // Stok
            if (filters.InStockOnly == true)
            {
                activeFilters.Add(new ActiveFilterViewModel
                {
                    Label = "Stok",
                    Value = "Stokta Var",
                    RemoveUrl = Url.Action("Index", new
                    {
                        filters.CategoryId,
                        filters.MinPrice,
                        filters.MaxPrice,
                        filters.Condition,
                        InStockOnly = (bool?)null,
                        filters.SortBy,
                        filters.SearchQuery,
                        PageNumber = 1
                    })
                });
            }

            // Arama
            if (!string.IsNullOrEmpty(filters.SearchQuery))
            {
                activeFilters.Add(new ActiveFilterViewModel
                {
                    Label = "Arama",
                    Value = filters.SearchQuery,
                    RemoveUrl = Url.Action("Index", new
                    {
                        filters.CategoryId,
                        filters.MinPrice,
                        filters.MaxPrice,
                        filters.Condition,
                        filters.InStockOnly,
                        filters.SortBy,
                        SearchQuery = (string?)null,
                        PageNumber = 1
                    })
                });
            }

            return activeFilters;
        }

        // ✅ ProductDetail (Aynı kalıyor)
        public async Task<IActionResult> ProductDetail(long id)
        {
            var productDto = await _productService.GetByIdProduct(id);
            var productImages = await _productImageService.GetByIdProductImagesAsync(id);
            var owner = await _accountService.FindById(productDto.OwnerId);

            var allProducts = (await _productService.GetAllProducts())
                .Where(p => !p.IsDeleted)
                .OrderBy(p => p.Id)
                .ToList();

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
                CategoryName = productDto.CategoryName,
                ImageUrl = productDto.ProductImages.FirstOrDefault(img => img.IsMain && !img.IsDeleted)?.ImageUrl
                    ?? "/template/assets/images/default-product.png",
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
            ViewBag.About = aboutDtos.Select(dto => new AboutViewModel
            {
                AboutId = dto.AboutId,
                Description = dto.Description,
                Address = dto.Address,
                Email = dto.Email,
                Phone = dto.Phone
            }).ToList();

            return View(viewModel);
        }
    }
}
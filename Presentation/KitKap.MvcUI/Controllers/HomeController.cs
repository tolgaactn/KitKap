using Kitkap.Entity.Services;
using Kitkap.Service.Services;
using KitKap.MvcUI.Models;
using KitKap.MvcUI.ViewModels.HomeViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace KitKap.MvcUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;

        public HomeController(
            ILogger<HomeController> logger,
            IProductService productService,
            ICategoryService categoryService)
        {
            _logger = logger;
            _productService = productService;
            _categoryService = categoryService;
        }

        public async Task<IActionResult> Index()
        {
            // 1. Tüm ürünleri çek
            var allProducts = (await _productService.GetAllProducts())
                .Where(p => !p.IsDeleted && p.Stock > 0)
                .ToList();

            // 2. Kategorileri çek
            var categories = (await _categoryService.GetAllCategories())
                .Where(c => !c.IsDeleted)
                .ToList();

            var viewModel = new HomePageViewModel();

            // 3. Kategori Slider'? doldur
            foreach (var category in categories.Take(5)) // ?lk 5 kategori
            {
                var categoryProducts = allProducts
                    .Where(p => p.CategoryId == category.Id)
                    .ToList();

                if (categoryProducts.Any())
                {
                    var featuredProduct = categoryProducts.First();

                    viewModel.CategorySlides.Add(new CategorySlideViewModel
                    {
                        CategoryId = category.Id,
                        CategoryName = category.Name,
                        ProductCount = categoryProducts.Count,
                        ImageUrl = $"/template/assets/images/demoes/demo25/slider/{GetCategoryImage(category.Name)}",
                        FeaturedProduct = new ProductCardViewModel
                        {
                            Id = featuredProduct.Id,
                            Name = featuredProduct.Name,
                            Price = featuredProduct.Price,
                            ImageUrl = featuredProduct.ProductImages?.FirstOrDefault(img => img.IsMain && !img.IsDeleted)?.ImageUrl
                                ?? "/template/assets/images/default-product.png",
                            CategoryName = category.Name
                        }
                    });
                }
            }

            // 4. Best Sellers - En yeni 8 ürün
            viewModel.BestSellers = allProducts
                .OrderByDescending(p => p.CreatedAt)
                .Take(8)
                .Select(p => new ProductCardViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    ImageUrl = p.ProductImages?.FirstOrDefault(img => img.IsMain && !img.IsDeleted)?.ImageUrl
                        ?? "/template/assets/images/default-product.png",
                    CategoryName = p.CategoryName,
                    IsHot = true
                }).ToList();

            // 5. Featured Books - Rastgele 6 ürün
            var random = new Random();
            viewModel.FeaturedBooks = allProducts
                .OrderBy(x => random.Next())
                .Take(6)
                .Select(p => new ProductCardViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    ImageUrl = p.ProductImages?.FirstOrDefault(img => img.IsMain && !img.IsDeleted)?.ImageUrl
                        ?? "/template/assets/images/default-product.png",
                    CategoryName = p.CategoryName,
                    IsHot = true
                }).ToList();

            // 6. Recent Books - Son eklenen 6 ürün
            viewModel.RecentBooks = allProducts
                .OrderByDescending(p => p.CreatedAt)
                .Take(6)
                .Select(p => new ProductCardViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    ImageUrl = p.ProductImages?.FirstOrDefault(img => img.IsMain && !img.IsDeleted)?.ImageUrl
                        ?? "/template/assets/images/default-product.png",
                    CategoryName = p.CategoryName
                }).ToList();

            return View(viewModel);
        }

        // Helper method - Kategori resimlerini e?le?tir
        private string GetCategoryImage(string categoryName)
        {
            var imageMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "Roman", "literature-fiction.jpg" },
                { "Bilim Kurgu", "scifi-fantasy.jpg" },
                { "Polisiye", "mystery-suspense.jpg" },
                { "Sanat", "arts-photography.jpg" },
                { "?? & Ekonomi", "business-investing.jpg" }
            };

            return imageMap.ContainsKey(categoryName)
                ? imageMap[categoryName]
                : "literature-fiction.jpg";
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
using Kitkap.Service.Dtos.AddressDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using KitKap.Service.Services;
using KitKap.MvcUI.Areas.Admin.ViewModels.ProductViewModels;
using Kitkap.Entity.Services;

namespace KitKap.MvcUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AllowAnonymous]
    [Route("Admin/[controller]/[action]/{id?}")]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;

        public ProductController(IProductService productService, ICategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.v1 = "Ana Sayfa";
            ViewBag.v2 = "Ürünler";
            ViewBag.v3 = "Ürün Listesi";
            ViewBag.v0 = "Ürün İşlemleri";

            var productDtos = await _productService.GetAllProducts();

            var viewModels = productDtos.Select(dto => new GetAllProductViewModel
            {
                Id = dto.Id,
                Name = dto.Name,
                CategoryId = dto.CategoryId,
                Description = dto.Description,
                Status = dto.Status,
                OwnerId = dto.OwnerId,
                Price = dto.Price,
                Stock = dto.Stock,
                IsDeleted = dto.IsDeleted,
                ImageUrl = dto.ProductImages.FirstOrDefault()?.ImageUrl ?? "/template/assets/images/default-product.png"
            }).ToList();

            return View(viewModels);
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var products = await _productService.GetAllProducts();
            ViewBag.Products = products;
            var categories = await _categoryService.GetAllCategories();
            ViewBag.Categories = categories;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateProductViewModel model, List<IFormFile> ProductImages)
        {
            


            if (ModelState.IsValid)
            {
                var product = new CreateProductDto
                {

                    Name = model.Name,
                    Description = model.Description,
                    CategoryId = model.CategoryId,
                    Status = model.Status,
                    OwnerId = model.OwnerId,
                    Price = model.Price,
                    Stock = model.Stock,
                    IsDeleted = model.IsDeleted
                    

                };
                await _productService.AddAsync(product);

                return RedirectToAction("Index");
            }

            var products = await _productService.GetAllProducts();
            ViewBag.Categories = products;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var dto = new RemoveProductDto { Id = id };
             
            await _productService.DeleteAsync(dto);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var productDto = await _productService.GetByIdProduct(id);

            if (productDto == null)
            {
                return NotFound();
            }

            // categoryDto'yu CategoryViewModel'e dönüştürüyoruz
            var model = new ProductViewModel
            {
                Id = productDto.Id,
                Name = productDto.Name,
                Description = productDto.Description,
                CategoryId = productDto.CategoryId,
                Status = productDto.Status,
                OwnerId = productDto.OwnerId,
                Price = productDto.Price,
                Stock = productDto.Stock,
                IsDeleted = productDto.IsDeleted
            };

            return View(model); // Düzenleme sayfasını gösterir
        }

        // Update (Düzenlenmiş kategoriyi kaydeder)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductViewModel model)
        {
            if (ModelState.IsValid)
            {

                var existingProduct = new UpdateProductDto
                {
                    Id = model.Id,
                    CategoryId = model.CategoryId,
                    Description = model.Description,
                    Status = model.Status,
                    IsDeleted= model.IsDeleted,
                    Name = model.Name,
                    OwnerId = model.OwnerId,
                    Price = model.Price,
                    Stock = model.Stock
                };


                await _productService.UpdateAsync(existingProduct); // Güncellemeyi servis üzerinden yap

                return RedirectToAction("Index");
            }

            // Model valid değilse, tekrar edit sayfasına dön
            return View(model);
        }

    }
}

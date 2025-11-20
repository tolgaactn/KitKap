using Kitkap.Service.Dtos.AddressDtos;
using Microsoft.AspNetCore.Mvc;
using KitKap.MvcUI.Areas.Admin.ViewModels.ProductViewModels;
using Kitkap.Entity.Services;
using Kitkap.Service.Services;
using Humanizer;
using Kitkap.Entity.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using KitKap.Service.Services.Interfaces;
using KitKap.MvcUI.Areas.Admin.ViewModels.ProductImagesViewModels;
using KitKap.Service.Dtos.ProductDtos;

namespace KitKap.MvcUI.Areas.Admin.Controllers
{
    [Route("Admin/Product")]
    public class ProductController : BaseAdminController
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IProductImageService _productImageService;

        public ProductController(
            IProductService productService,
            ICategoryService categoryService,
            IProductImageService productImageService)
        {
            _productService = productService;
            _categoryService = categoryService;
            _productImageService = productImageService;
        }

        [Route("")]
        [Route("Index")]
        public async Task<IActionResult> Index(string? search)
        {
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
                ImageUrl = dto.ProductImages.FirstOrDefault(img => img.IsMain && !img.IsDeleted)?.ImageUrl
                    ?? "/template/assets/images/default-product.png",
                CategoryName = dto.CategoryName
            }).ToList();

            ViewData["TotalCount"] = viewModels.Count();

            if (!string.IsNullOrWhiteSpace(search))
            {
                viewModels = viewModels.Where(a =>
                    (a.Name != null && a.Name.ToLower().Contains(search.ToLower().Trim())) ||
                    (int.TryParse(search.Trim(), out int productId) && a.Id == productId)
                ).ToList();
            }

            return View(viewModels);
        }

        [HttpGet]
        [Route("Create")]
        public async Task<IActionResult> Create()
        {
            var products = await _productService.GetAllProducts();
            ViewBag.Products = products;
            var categories = await _categoryService.GetAllCategories();
            ViewBag.Categories = categories;
            return View();
        }

        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Create(CreateProductViewModel model)
        {
            if (ModelState.IsValid)
            {
                var productImageDtos = new List<CreateProductImageDto>();

                if (model.ProductImageFiles != null && model.ProductImageFiles.Any())
                {
                    for (int i = 0; i < model.ProductImageFiles.Count; i++)
                    {
                        var file = model.ProductImageFiles[i];
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/products", fileName);

                        using (var stream = new FileStream(uploadPath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        productImageDtos.Add(new CreateProductImageDto
                        {
                            ImageUrl = "/uploads/products/" + fileName,
                            AltText = "",
                            IsMain = model.MainImageIndex == i
                        });
                    }
                }

                var product = new CreateProductDto
                {
                    Name = model.Name,
                    Description = model.Description,
                    CategoryId = model.CategoryId,
                    Status = model.Status,
                    OwnerId = model.OwnerId,
                    Price = model.Price,
                    Stock = model.Stock,
                    IsDeleted = model.IsDeleted,
                    ProductImages = productImageDtos
                };

                await _productService.AddAsync(product);
                SetSuccessMessage("Ürün başarıyla eklendi!");
                return RedirectToAction("Index");
            }

            ViewBag.Categories = await _categoryService.GetAllCategories();
            return View(model);
        }

        [HttpPost]
        [Route("Delete/{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var dto = new RemoveProductDto { Id = id };
            await _productService.DeleteAsync(dto);
            SetSuccessMessage("Ürün başarıyla silindi!");
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Route("Edit/{id}")]
        public async Task<IActionResult> Edit(long id)
        {
            var productDto = await _productService.GetByIdProduct(id);
            var productImages = await _productImageService.GetByIdProductImagesAsync(id);

            if (productDto == null)
            {
                return NotFound();
            }

            var model = new EditProductViewModel
            {
                Id = productDto.Id,
                Name = productDto.Name,
                Description = productDto.Description,
                Price = productDto.Price,
                Stock = productDto.Stock,
                CategoryId = productDto.CategoryId,
                Status = productDto.Status,
                IsDeleted = productDto.IsDeleted,
                OwnerId = productDto.OwnerId,
                ExistingImages = productImages
                    .Where(x => !x.IsDeleted)
                    .Select(x => new ExistingProductImageViewModel
                    {
                        Id = x.Id,
                        ImageUrl = x.ImageUrl,
                        AltText = x.AltText,
                        IsMain = x.IsMain
                    })
                    .ToList()
            };

            ViewBag.Categories = new SelectList(await _categoryService.GetAllCategories(), "Id", "Name");
            return View(model);
        }

        [HttpPost]
        [Route("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditProductViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = new SelectList(await _categoryService.GetAllCategories(), "Id", "Name");
                return View(model);
            }

            var updateProductDto = new UpdateProductDto
            {
                Id = model.Id,
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                Stock = model.Stock,
                CategoryId = model.CategoryId,
                Status = model.Status,
                IsDeleted = model.IsDeleted,
                OwnerId = model.OwnerId
            };

            await _productService.UpdateAsync(updateProductDto);

            if (model.SelectedMainImageId.HasValue)
            {
                await _productImageService.SetMainImageAsync(model.SelectedMainImageId.Value, model.Id);
            }

            if (model.ImagesToDelete != null && model.ImagesToDelete.Any())
            {
                await _productImageService.MarkAsDeletedAsync(model.ImagesToDelete);
            }

            if (model.NewProductImages != null && model.NewProductImages.Any())
            {
                await _productImageService.AddImagesAsync(model.Id, model.NewProductImages);
            }

            SetSuccessMessage("Ürün başarıyla güncellendi!");
            return RedirectToAction(nameof(Index));
        }

        [Route("Detail/{id}")]
        public async Task<IActionResult> ProductDetail(long id)
        {
            var productDto = await _productService.GetByIdProduct(id);
            var productImages = await _productImageService.GetByIdProductImagesAsync(id);

            if (productDto == null)
            {
                return NotFound();
            }

            var model = new AdminProductDetailViewModel
            {
                Id = productDto.Id,
                Name = productDto.Name,
                Description = productDto.Description,
                Price = productDto.Price,
                Stock = productDto.Stock,
                CategoryName = productDto.CategoryName,
                ImageUrl = productDto.ProductImages.FirstOrDefault(img => img.IsMain && !img.IsDeleted)?.ImageUrl
                    ?? "/template/assets/images/default-product.png",
                ImageUrls = productImages.Where(img => !img.IsDeleted).Select(img => img.ImageUrl).ToList()
            };

            return View(model);
        }
    }
}
using Kitkap.Entity.Entities;
using Kitkap.Entity.Services;
using Kitkap.Service.Dtos.AddressDtos;
using KitKap.MvcUI.Areas.Admin.ViewModels.CategoryViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace KitKap.MvcUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AllowAnonymous]
	[Route("Admin/[controller]/[action]/{id?}")]
    public class CategoryController : Controller
	{
		public readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }
		

        public async Task <IActionResult> Index()
		{
			ViewBag.v1 = "Ana Sayfa";
			ViewBag.v2 = "Kategoriler";
			ViewBag.v3 = "Tüm Kategoriler";
			ViewBag.v0 = "Kategori İşlemleri";

            var categoryDtos = await _categoryService.GetAllCategories();

			var viewModels = categoryDtos.Select(dto => new CategoryViewModel
			{
				Id = dto.Id,
				Name = dto.Name,
				CreatedDate = dto.CreatedDate,
				Description = dto.Description,
				ParentCategoryId = dto.ParentCategoryId,
                IsDeleted = dto.IsDeleted
			}).ToList();

            return View(viewModels);
        }
        [HttpGet]
		public async Task <IActionResult> Create()
		{
            var categories = await _categoryService.GetAllCategories();
			ViewBag.Categories = categories;
            return View();
        }

        [HttpPost]
		public async Task <IActionResult> Create(CreateCategoryViewModel model)
		{

            if (ModelState.IsValid) { 
			var category = new CreateCategoryDto { 
				
				Name = model.Name,
				Description = model.Description,
				ParentCategoryId = model.ParentCategoryId,
				CreatedDate = DateTime.Now 
			
			};
			await _categoryService.AddAsync(category);

			return RedirectToAction("Index");
            }

            var categories = await _categoryService.GetAllCategories();
            ViewBag.Categories = categories;

            return View(model);
        }

		[HttpPost]
		public async Task<IActionResult> Delete(int id)
		{
			await _categoryService.DeleteAsync(id);

			return RedirectToAction("Index");
		}

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var categoryDto = await _categoryService.GetByIdCategory(id);

            if (categoryDto == null)
            {
                return NotFound();
            }

            // categoryDto'yu CategoryViewModel'e dönüştürüyoruz
            var model = new CategoryViewModel
            {
                Id = categoryDto.Id,
                Name = categoryDto.Name,
                Description = categoryDto.Description,
                ParentCategoryId = categoryDto.ParentCategoryId,
                CreatedDate = categoryDto.CreatedDate
            };

            return View(model); // Düzenleme sayfasını gösterir
        }

        // Update (Düzenlenmiş kategoriyi kaydeder)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                
                var existingCategory = new UpdateCategoryDto
                {
                    Id = model.Id,
                    CreatedDate = model.CreatedDate,
                    Description = model.Description,
                    Name = model.Name,
                    ParentCategoryId = model.ParentCategoryId
                };


                await _categoryService.UpdateAsync(existingCategory); // Güncellemeyi servis üzerinden yap

                return RedirectToAction("Index");
            }

            // Model valid değilse, tekrar edit sayfasına dön
            return View(model);
        }



    }
}

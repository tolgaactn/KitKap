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

        [HttpGet]
        public async Task <IActionResult> Index(string? search)
		{

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

            ViewData["TotalCount"] = viewModels.Count();

            if (!string.IsNullOrWhiteSpace(search))
            {
                viewModels = viewModels.Where(a => (a.Name != null && a.Name.ToLower().Contains(search.ToLower().Trim())) || (int.TryParse(search.Trim(), out int categoryId) && a.Id == categoryId)).ToList();
            }

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
                CreatedDate = categoryDto.CreatedDate,
                IsDeleted = categoryDto.IsDeleted
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
                    ParentCategoryId = model.ParentCategoryId,
                    IsDeleted = model.IsDeleted,
                };


                await _categoryService.UpdateAsync(existingCategory); // Güncellemeyi servis üzerinden yap

                return RedirectToAction("Index");
            }

            // Model valid değilse, tekrar edit sayfasına dön
            return View(model);
        }



    }
}

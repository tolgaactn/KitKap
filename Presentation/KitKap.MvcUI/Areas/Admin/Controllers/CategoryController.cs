using Kitkap.Entity.Entities;
using Kitkap.Entity.Services;
using Kitkap.Entity.ViewModels.CategoryViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace KitKap.MvcUI.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class CategoryController : Controller
	{
		public readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }
		[AllowAnonymous]

        public async Task <IActionResult> Index()
		{
			ViewBag.v1 = "Ana Sayfa";
			ViewBag.v2 = "Kategoriler";
			ViewBag.v3 = "Tüm Kategoriler";
			ViewBag.v0 = "Kategori İşlemleri";
			var categories = await _categoryService.GetAllCategories();

            return View(categories);
		}

		[HttpGet]
		public async Task <IActionResult> Create()
		{
            var categories = await _categoryService.GetAllCategories();
            var categoryList = categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToList();

            var model = new CreateCategoryViewModel
            {
                //Categories = categoryList
            };

            return View(model);
        }


		[HttpPost]
		public async Task <IActionResult> Create(CreateCategoryViewModel model)
		{
			if (ModelState.IsValid) { 
			var category = new CreateCategoryViewModel { 
				
				Name = model.Name,
				Description = model.Description,
				ParentCategoryId = model.ParentCategoryId,
				CreatedDate = DateTime.Now 
			
			};
			await _categoryService.AddAsync(category);

			return RedirectToAction("Index");
            }
            return View(model);
        }
	}
}

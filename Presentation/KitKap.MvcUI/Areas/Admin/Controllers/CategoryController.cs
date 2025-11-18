using Kitkap.Entity.Entities;
using Kitkap.Entity.Services;
using Kitkap.Service.Dtos.AddressDtos;
using KitKap.MvcUI.Areas.Admin.ViewModels.CategoryViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace KitKap.MvcUI.Areas.Admin.Controllers
{
    [Route("Admin/Category")]
    public class CategoryController : BaseAdminController
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        [Route("")]
        [Route("Index")]
        public async Task<IActionResult> Index(string? search)
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
                viewModels = viewModels.Where(a =>
                    (a.Name != null && a.Name.ToLower().Contains(search.ToLower().Trim())) ||
                    (int.TryParse(search.Trim(), out int categoryId) && a.Id == categoryId)
                ).ToList();
            }

            return View(viewModels);
        }

        [HttpGet]
        [Route("Create")]
        public async Task<IActionResult> Create()
        {
            var categories = await _categoryService.GetAllCategories();
            ViewBag.Categories = categories;
            return View();
        }

        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Create(CreateCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var category = new CreateCategoryDto
                {
                    Name = model.Name,
                    Description = model.Description,
                    ParentCategoryId = model.ParentCategoryId,
                    CreatedDate = DateTime.Now
                };

                await _categoryService.AddAsync(category);
                SetSuccessMessage("Kategori başarıyla eklendi!");
                return RedirectToAction("Index");
            }

            var categories = await _categoryService.GetAllCategories();
            ViewBag.Categories = categories;

            return View(model);
        }

        [HttpPost]
        [Route("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _categoryService.DeleteAsync(id);
            SetSuccessMessage("Kategori başarıyla silindi!");
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Route("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var categoryDto = await _categoryService.GetByIdCategory(id);

            if (categoryDto == null)
            {
                return NotFound();
            }

            var model = new CategoryViewModel
            {
                Id = categoryDto.Id,
                Name = categoryDto.Name,
                Description = categoryDto.Description,
                ParentCategoryId = categoryDto.ParentCategoryId,
                CreatedDate = categoryDto.CreatedDate,
                IsDeleted = categoryDto.IsDeleted
            };

            return View(model);
        }

        [HttpPost]
        [Route("Edit/{id}")]
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

                await _categoryService.UpdateAsync(existingCategory);
                SetSuccessMessage("Kategori başarıyla güncellendi!");
                return RedirectToAction("Index");
            }

            return View(model);
        }
    }
}
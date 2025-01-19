using Kitkap.Entity.Services;
using Kitkap.Entity.ViewModels.BookViewModels;
using Kitkap.Entity.ViewModels.CategoryViewModels;
using KitKap.Service.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kitkap.WebMvcUI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategorys()
        {
            var categories = await _categoryService.GetAllCategories();
            return Ok(categories);
        }

        [HttpGet("id")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var category = await _categoryService.GetByIdCategory(id);
            return Ok(category);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory(CreateCategoryViewModel model)
        {
            await _categoryService.AddAsync(model);
            return Ok(model.Name);
        }
        [HttpPut]
        public async Task<IActionResult> UpdateCategory(UpdateCategoryViewModel model)
        {
            await _categoryService.UpdateAsync(model);
            return Ok(model.Name);
        }
        [HttpDelete]
        public async Task<IActionResult> RemoveCategory(RemoveCategoryViewModel model)
        {
            await _categoryService.DeleteAsync(model);
            return Ok("Kategori Güncellendi");
        }
    }
}

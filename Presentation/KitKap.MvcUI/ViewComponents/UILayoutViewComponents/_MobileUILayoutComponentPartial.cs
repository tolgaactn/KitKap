using Kitkap.Entity.Services;
using Microsoft.AspNetCore.Mvc;

namespace KitKap.MvcUI.ViewComponents.UILayoutViewComponents
{
    public class _MobileUILayoutComponentPartial : ViewComponent
    {
        private readonly ICategoryService _categoryService;

        public _MobileUILayoutComponentPartial(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var categories = await _categoryService.GetAllCategories();

            // Sadece ana kategorileri al (ParentCategoryId == null)
            var mainCategories = categories
                .Where(c => !c.IsDeleted && c.ParentCategoryId == null)
                .OrderBy(c => c.Name)
                .ToList();

            return View(mainCategories);
        }
    }
}

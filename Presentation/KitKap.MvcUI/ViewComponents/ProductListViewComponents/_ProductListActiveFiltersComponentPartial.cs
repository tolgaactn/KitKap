using KitKap.MvcUI.ViewModels.ProductListViewModels;
using Microsoft.AspNetCore.Mvc;

namespace KitKap.MvcUI.ViewComponents
{
    public class _ProductListActiveFiltersComponentPartial : ViewComponent
    {
        public IViewComponentResult Invoke(ProductListPageViewModel model)
        {
            return View(model);
        }
    }
}
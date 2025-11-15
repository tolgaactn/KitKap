using KitKap.MvcUI.ViewModels.ProductListViewModels;
using Microsoft.AspNetCore.Mvc;

namespace KitKap.MvcUI.ViewComponents.ProductListViewComponents
{
    public class _ProductListStickyHeaderComponentPartial : ViewComponent
    {
        public IViewComponentResult Invoke(ProductListPageViewModel model)
        {
            return View();
        }
    }
}

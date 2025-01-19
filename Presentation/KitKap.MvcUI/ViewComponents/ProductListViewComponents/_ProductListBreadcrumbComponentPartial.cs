using Microsoft.AspNetCore.Mvc;

namespace KitKap.MvcUI.ViewComponents.ProductListViewComponents
{
    public class _ProductListBreadcrumbComponentPartial : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}

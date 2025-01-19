using Microsoft.AspNetCore.Mvc;

namespace KitKap.MvcUI.ViewComponents.ProductListViewComponents
{
    public class _ProductListStickyHeaderComponentPartial : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}

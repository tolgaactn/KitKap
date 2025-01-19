using Microsoft.AspNetCore.Mvc;

namespace KitKap.MvcUI.ViewComponents.ProductListViewComponents
{
    public class _ProductListPriceFilterComponentPartial : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}

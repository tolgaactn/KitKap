using Microsoft.AspNetCore.Mvc;

namespace KitKap.MvcUI.ViewComponents.ProductDetailViewComponents
{
    public class _ProductDetailSidebarComponentPartial : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}

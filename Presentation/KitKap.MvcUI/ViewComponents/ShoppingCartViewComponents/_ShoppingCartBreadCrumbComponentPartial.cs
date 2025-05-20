using Microsoft.AspNetCore.Mvc;

namespace KitKap.MvcUI.ViewComponents.ShoppingCartViewComponents
{
    public class _ShoppingCartBreadCrumbComponentPartial : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}

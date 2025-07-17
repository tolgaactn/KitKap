using Microsoft.AspNetCore.Mvc;

namespace KitKap.MvcUI.ViewComponents.ShoppingCartViewComponents
{
    public class _ShoppingCartTableHeadComponentPartial : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}

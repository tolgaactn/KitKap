using Microsoft.AspNetCore.Mvc;

namespace KitKap.MvcUI.ViewComponents.UILayoutViewComponents
{
    public class _NavbarUILayoutComponentPartial : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}

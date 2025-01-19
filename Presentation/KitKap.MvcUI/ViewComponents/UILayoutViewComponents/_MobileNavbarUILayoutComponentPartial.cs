using Microsoft.AspNetCore.Mvc;

namespace KitKap.MvcUI.ViewComponents.UILayoutViewComponents
{
    public class _MobileNavbarUILayoutComponentPartial:ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}

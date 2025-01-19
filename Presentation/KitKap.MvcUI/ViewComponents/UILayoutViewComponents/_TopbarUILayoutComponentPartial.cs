using Microsoft.AspNetCore.Mvc;

namespace KitKap.MvcUI.ViewComponents.UILayoutViewComponents
{
    public class _TopbarUILayoutComponentPartial : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}

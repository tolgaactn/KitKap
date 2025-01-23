using Microsoft.AspNetCore.Mvc;

namespace KitKap.MvcUI.Areas.Admin.ViewComponents.AdminLayoutViewComponents
{
    public class _AdminLayoutPreloadComponentPartial : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}

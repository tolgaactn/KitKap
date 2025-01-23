using Microsoft.AspNetCore.Mvc;

namespace KitKap.MvcUI.Areas.Admin.ViewComponents.AdminLayoutViewComponents
{
    public class _AdminLayoutJsComponentPartial : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}

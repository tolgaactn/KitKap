using Microsoft.AspNetCore.Mvc;

namespace KitKap.MvcUI.Areas.Admin.ViewComponents.AdminLayoutViewComponents
{
    public class _AdminLayoutSectionMenuLeftComponentPartial : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}

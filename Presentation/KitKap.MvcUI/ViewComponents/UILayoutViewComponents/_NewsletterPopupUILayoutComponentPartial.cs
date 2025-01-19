using Microsoft.AspNetCore.Mvc;

namespace KitKap.MvcUI.ViewComponents.UILayoutViewComponents
{
    public class _NewsletterPopupUILayoutComponentPartial: ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}

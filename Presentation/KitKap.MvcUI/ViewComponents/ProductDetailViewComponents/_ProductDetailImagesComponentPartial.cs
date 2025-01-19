using Microsoft.AspNetCore.Mvc;

namespace KitKap.MvcUI.ViewComponents.ProductDetailViewComponents
{
    public class _ProductDetailImagesComponentPartial : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}

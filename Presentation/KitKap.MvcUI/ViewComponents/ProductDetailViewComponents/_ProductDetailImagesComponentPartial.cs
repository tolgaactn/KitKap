using KitKap.MvcUI.ViewModels.ProductDetailViewModels;
using Microsoft.AspNetCore.Mvc;

namespace KitKap.MvcUI.ViewComponents.ProductDetailViewComponents
{
    public class _ProductDetailImagesComponentPartial : ViewComponent
    {
        public IViewComponentResult Invoke(ProductDetailViewModel model)
        {
            return View();
        }
    }
}

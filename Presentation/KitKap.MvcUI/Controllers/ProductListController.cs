using Microsoft.AspNetCore.Mvc;

namespace KitKap.MvcUI.Controllers
{
    public class ProductListController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult ProductDetail()
        {
            return View();
        }
    }
}

using Microsoft.AspNetCore.Mvc;

namespace KitKap.MvcUI.Controllers
{
    public class ShoppingCartController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

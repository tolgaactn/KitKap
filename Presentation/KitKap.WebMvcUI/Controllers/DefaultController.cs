using Microsoft.AspNetCore.Mvc;

namespace KitKap.WebMvcUI.Controllers
{
    public class DefaultController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

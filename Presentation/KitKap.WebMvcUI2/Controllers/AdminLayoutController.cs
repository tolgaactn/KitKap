using Microsoft.AspNetCore.Mvc;

namespace KitKap.WebMvcUI.Controllers
{
    public class AdminLayoutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

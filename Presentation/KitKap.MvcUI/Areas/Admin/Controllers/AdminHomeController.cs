using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KitKap.MvcUI.Areas.Admin.Controllers
{
    public class AdminHomeController : Controller
    {
        [Area("Admin")]
        [AllowAnonymous]
        [Route("Admin/[controller]/[action]/{id?}")]
        public IActionResult Index()
        {
            return View();
        }
    }
}

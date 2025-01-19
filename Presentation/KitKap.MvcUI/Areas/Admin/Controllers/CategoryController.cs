using Microsoft.AspNetCore.Mvc;

namespace KitKap.MvcUI.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class CategoryController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}

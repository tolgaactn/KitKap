using Microsoft.AspNetCore.Mvc;

namespace KitKap.MvcUI.ViewComponents.ShoppingCartViewComponents
{
	public class _ShoppingCartDetailComponentPartial : ViewComponent
	{
		public IViewComponentResult Invoke()
		{
			return View();
		}
	}
}

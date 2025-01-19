using Microsoft.AspNetCore.Mvc;

namespace KitKap.MvcUI.ViewComponents.ShoppingCartViewComponents
{
	public class _ShoppingCartProductListComponentPartial : ViewComponent
	{
		public IViewComponentResult Invoke()
		{
			return View();
		}
	}
}

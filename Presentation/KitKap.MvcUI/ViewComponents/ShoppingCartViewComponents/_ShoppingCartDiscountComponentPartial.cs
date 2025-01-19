using Microsoft.AspNetCore.Mvc;

namespace KitKap.MvcUI.ViewComponents.ShoppingCartViewComponents
{
	public class _ShoppingCartDiscountComponentPartial : ViewComponent
	{
		public IViewComponentResult Invoke()
		{
			return View();
		}
	}
}

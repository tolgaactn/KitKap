using AutoMapper;
using KitKap.MvcUI.ViewModels.ShoppingCartViewModels;
using KitKap.Service.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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

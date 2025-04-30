using KitKap.MvcUI.Areas.Admin.ViewModels.AboutViewModels;
using KitKap.MvcUI.Models;
using KitKap.MvcUI.ViewModels.ProductDetailViewModels;
using KitKap.Service.Dtos.AboutDtos;
using KitKap.Service.Dtos.ProductDtos;
using KitKap.Service.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace KitKap.MvcUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IAboutService _aboutService;

        public HomeController(ILogger<HomeController> logger, IAboutService aboutService)
        {
            _logger = logger;
            _aboutService = aboutService;
        }

        public async Task<IActionResult> Index()
        {
            var aboutDtos = await _aboutService.GetAllAboutAsync();

                var viewModel = aboutDtos.Select(aboutDto => new AboutViewModel
                {
                    AboutId = aboutDto.AboutId,
                    Description = aboutDto.Description,
                    Address = aboutDto.Address,
                    Email = aboutDto.Email,
                    Phone = aboutDto.Phone
                }).ToList();

                ViewBag.About = viewModel;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }
        public IActionResult Register()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

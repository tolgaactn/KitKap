using Kitkap.Entity.Services;
using Kitkap.Service.Dtos.AddressDtos;
using KitKap.MvcUI.ViewModels.AccountViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KitKap.MvcUI.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var loginDto = new LoginUserDto
            {
                Email = model.Email,
                Password = model.Password,
                RememberMe = model.RememberMe
            };

            var authResponse = await _accountService.LoginAsync(loginDto);

            if (!authResponse.IsSuccessful)
            {
                ModelState.AddModelError("", "Giriş başarısız.");
                return View(model);
            }

            // Kullanıcı başarılı giriş yaptıysa rollerini kontrol et
            var roles = await _accountService.GetRolesAsync(model.Email);

            if (roles.Contains("Admin"))
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });

            if (roles.Contains("Kurumsal"))
                return RedirectToAction("Index", "CorporateDashboard");

            if (roles.Contains("BireyselMusteri"))
                return RedirectToAction("Index", "Home");

            // Rolü yoksa fallback
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var dto = new RegisterUserDto
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Password = model.Password,
                UserName = model.UserName
            };

            var result = await _accountService.CreateUserAsync(dto);

            if (result == "OK")
                return RedirectToAction("Login");

            ModelState.AddModelError("", result);
            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _accountService.LogoutAsync(); // Token sil veya cookie temizle
            return RedirectToAction("Login");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}


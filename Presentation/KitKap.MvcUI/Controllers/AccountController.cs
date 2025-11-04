using Kitkap.Entity.Services;
using Kitkap.Service.Dtos.AddressDtos;
using KitKap.MvcUI.ViewModels.AccountViewModels;
using KitKap.Service.Services.Interfaces; // ✅ ShoppingCartService için
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KitKap.MvcUI.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly IShoppingCartService _shoppingCartService; // ✅ YENİ

        public AccountController(
            IAccountService accountService,
            IShoppingCartService shoppingCartService) // ✅ YENİ
        {
            _accountService = accountService;
            _shoppingCartService = shoppingCartService; // ✅ YENİ
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
                RememberMe = model.RememberMe,
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
            };

            var authResponse = await _accountService.LoginAsync(loginDto);
            if (!authResponse.IsSuccessful)
            {
                foreach (var error in authResponse.Errors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }
                return View(model);
            }

            // ✅✅✅ LOGIN SONRASI SEPET MİGRATION ✅✅✅
            try
            {
                var guestId = CookieHelper.GetGuestId(HttpContext);
                if (!string.IsNullOrEmpty(guestId))
                {
                    var userInfo = await _accountService.GetUserByEmailAsync(model.Email);
                    if (userInfo != null)
                    {
                        await _shoppingCartService.MergeGuestCartToUserAsync(userInfo.Id, guestId);
                        CookieHelper.RemoveGuestId(HttpContext);
                    }
                }
            }
            catch (Exception ex)
            {
                // Migration hatası login'i engellemez, sadece log'la
                // Logger varsa: _logger.LogError(ex, "Sepet migration hatası");
                Console.WriteLine($"Sepet migration hatası: {ex.Message}");
            }
            // ✅✅✅ MİGRATION BİTTİ ✅✅✅

            // Kullanıcı başarılı giriş yaptıysa rollerini kontrol et
            var roles = await _accountService.GetRolesAsync(model.Email);
            if (roles.Contains("Admin"))
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            if (roles.Contains("Kurumsal"))
                return RedirectToAction("Index", "CorporateDashboard");
            if (roles.Contains("BireyselMusteri"))
                return RedirectToAction("Index", "Home");

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
            await _accountService.LogoutAsync();
            return RedirectToAction("Login");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
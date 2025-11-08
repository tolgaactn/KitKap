using Kitkap.Entity.Services;
using Kitkap.Service.Dtos.AddressDtos;
using Kitkap.Service.Dtos.UserDtos;
using KitKap.MvcUI.ViewModels.AccountViewModels;
using KitKap.Service.Services.Concretes;
using KitKap.Service.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace KitKap.MvcUI.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IAddressService _addressService;

        public AccountController(
            IAccountService accountService,
            IShoppingCartService shoppingCartService,
            IAddressService addressService)
        {
            _accountService = accountService;
            _shoppingCartService = shoppingCartService;
            _addressService = addressService;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            // Zaten giriş yapmışsa
            if (User.Identity?.IsAuthenticated ?? false)
            {
                return LocalRedirect(returnUrl ?? "/");
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

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

            // ✅ LOGIN SONRASI SEPET MIGRATION
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
                Console.WriteLine($"Sepet migration hatası: {ex.Message}");
            }

            // ✅ ROLE BAZLI YÖNLENDİRME
            var roles = await _accountService.GetRolesAsync(model.Email);

            if (roles.Contains("Admin"))
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });

            if (roles.Contains("Kurumsal"))
                return RedirectToAction("Index", "CorporateDashboard");

            // ✅ PROFESYONEL: ReturnUrl varsa oraya, yoksa Home'a
            return LocalRedirect(returnUrl ?? Url.Action("Index", "Home"));
        }

        [HttpGet]
        public IActionResult Register(string? returnUrl = null)
        {
            // Zaten giriş yapmışsa
            if (User.Identity?.IsAuthenticated ?? false)
            {
                return LocalRedirect(returnUrl ?? "/");
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

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
            {
                // ✅ Kayıt sonrası otomatik giriş yap
                var loginDto = new LoginUserDto
                {
                    Email = model.Email,
                    Password = model.Password,
                    RememberMe = false,
                    IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
                };

                var authResponse = await _accountService.LoginAsync(loginDto);

                if (authResponse.IsSuccessful)
                {
                    // ✅ Guest sepetini merge et
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
                        Console.WriteLine($"Sepet migration hatası: {ex.Message}");
                    }

                    TempData["Success"] = $"Hoş geldiniz! Hesabınız başarıyla oluşturuldu.";

                    // ✅ ReturnUrl varsa oraya, yoksa Home'a
                    return LocalRedirect(returnUrl ?? Url.Action("Index", "Home"));
                }
            }

            ModelState.AddModelError("", result);
            return View(model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {

            await _accountService.LogoutAsync();
            Response.Cookies.Delete("GuestId");

            // ✅ TempData'yı Home'da göstermek için Home'a yönlendir
            TempData["Success"] = "Başarıyla çıkış yaptınız. Görüşmek üzere! 👋";

            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var profileDto = await _accountService.GetUserProfileAsync(userId);

            if(profileDto == null)
            {
                TempData["Error"] = "Profil bilgilerinize erişilemedi.";
                return RedirectToAction("Index", "Home");
            }
            var addresses = await _addressService.GetByUserIdAsync(userId);

            var viewModel = new ProfileViewModel
            {
                Id = profileDto.Id,
                FirstName = profileDto.FirstName,
                LastName = profileDto.LastName,
                Email = profileDto.Email,
                PhoneNumber = profileDto.PhoneNumber,
                UserName = profileDto.UserName,
                Balance = profileDto.Balance,
                Addresses = addresses.ToList()
            };

            return View(viewModel);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UpdateProfile(ProfileViewModel model)
        {
            if(!ModelState.IsValid)
            {
                return View("Profile", model);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if(model.Id != userId)
            {
                TempData["Error"] = "Yetkisiz işlem!";
                return RedirectToAction("Profile");
            }
            try
            {
                var updateDto = new UpdateProfileDto
                {
                    Id = model.Id,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    PhoneNumber = model.PhoneNumber,
                    UserName = model.UserName
                };

                var result = await _accountService.UpdateUserProfileAsync(updateDto);

                if (result)
                {
                    TempData["Success"] = "Profil bilgileriniz başarıyla güncellendi! ✅";
                    return RedirectToAction("Profile");
                }
                else
                {
                    TempData["Error"] = "Profil güncelleme başarısız.";
                    return View("Profile", model);
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                {
                    return View("Profile", model);
                }
            }
        }
        /// <summary>
        /// Şifre değiştirme sayfasını göster (GET)
        /// </summary>
        [Authorize]
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        /// <summary>
        /// Şifre değiştir (POST)
        /// </summary>
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            // ✅ ÖNCELİK: Manuel şifre kontrolü
            if (model.NewPassword != model.ConfirmPassword)
            {
                TempData["Error"] = "Yeni şifreler birbiriyle uyuşmuyor! ❌";
                TempData["ActiveTab"] = "password-tab";
                return RedirectToAction("Profile");
            }

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Lütfen tüm alanları doğru şekilde doldurunuz.";
                TempData["ActiveTab"] = "password-tab";
                return RedirectToAction("Profile");
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            try
            {
                var changePasswordDto = new ChangePasswordDto
                {
                    UserId = userId,
                    CurrentPassword = model.CurrentPassword,
                    NewPassword = model.NewPassword,
                    ConfirmPassword = model.ConfirmPassword
                };

                var result = await _accountService.ChangePasswordAsync(changePasswordDto);

                if (result)
                {
                    TempData["Success"] = "Şifreniz başarıyla değiştirildi! 🔒";
                    TempData["ActiveTab"] = "profile-tab";
                    return RedirectToAction("Profile");
                }
                else
                {
                    TempData["Error"] = "Mevcut şifreniz hatalı. Lütfen kontrol edip tekrar deneyin.";
                    TempData["ActiveTab"] = "password-tab";
                    return RedirectToAction("Profile");
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Şifre değiştirme sırasında bir hata oluştu: " + ex.Message;
                TempData["ActiveTab"] = "password-tab";
                return RedirectToAction("Profile");
            }
        }
    }
}
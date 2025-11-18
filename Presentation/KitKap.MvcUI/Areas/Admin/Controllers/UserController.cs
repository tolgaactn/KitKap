using KitKap.MvcUI.Areas.Admin.ViewModels.UserViewModels;
using KitKap.Service.Services.Interfaces;
using KitKap.DataAccess.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace KitKap.MvcUI.Areas.Admin.Controllers
{
    [Route("Admin/User")]
    public class UserController : BaseAdminController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IOrderService _orderService;

        public UserController(
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IOrderService orderService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _orderService = orderService;
        }

        [Route("")]
        [Route("Index")]
        public async Task<IActionResult> Index(string? role, string? search)
        {
            var allUsers = _userManager.Users.ToList();
            var userViewModels = new List<UserListViewModel>();

            foreach (var user in allUsers)
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                userViewModels.Add(new UserListViewModel
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Balance = user.Balance,
                    Roles = userRoles.ToList(),
                    CreatedDate = user.CreatedDate,
                    IsLocked = await _userManager.IsLockedOutAsync(user)
                });
            }

            // Filtreleme - Role
            if (!string.IsNullOrEmpty(role) && role != "All")
            {
                userViewModels = userViewModels.Where(u => u.Roles.Contains(role)).ToList();
            }

            // Filtreleme - Search
            if (!string.IsNullOrEmpty(search))
            {
                userViewModels = userViewModels.Where(u =>
                    u.FirstName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    u.LastName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    u.Email.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    u.UserName.Contains(search, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }

            // Tarihe göre sırala (en yeni üstte)
            userViewModels = userViewModels.OrderByDescending(u => u.CreatedDate).ToList();

            ViewData["TotalCount"] = userViewModels.Count;
            ViewData["SelectedRole"] = role ?? "All";
            ViewData["SearchQuery"] = search;

            // Rol sayıları
            ViewData["AdminCount"] = userViewModels.Count(u => u.Roles.Contains("Admin"));
            ViewData["KurumsalCount"] = userViewModels.Count(u => u.Roles.Contains("KurumsalMusteri"));
            ViewData["BireyselCount"] = userViewModels.Count(u => u.Roles.Contains("BireyselMusteri"));

            return View(userViewModels);
        }

        [Route("Detail/{id}")]
        public async Task<IActionResult> Detail(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                SetErrorMessage("Kullanıcı bulunamadı!");
                return RedirectToAction("Index");
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var userOrders = await _orderService.GetOrdersByUserAsync(id);

            var viewModel = new UserDetailViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Balance = user.Balance,
                Roles = userRoles.ToList(),
                CreatedDate = user.CreatedDate,
                EmailConfirmed = user.EmailConfirmed,
                PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                TwoFactorEnabled = user.TwoFactorEnabled,
                LockoutEnd = user.LockoutEnd?.DateTime,
                LockoutEnabled = user.LockoutEnabled,
                AccessFailedCount = user.AccessFailedCount,
                TotalOrders = userOrders.Count(),
                TotalSpent = userOrders.Sum(o => o.TotalAmount),
                RecentOrders = userOrders.OrderByDescending(o => o.CreatedAt).Take(5).Select(o => new UserOrderSummaryViewModel
                {
                    OrderId = o.Id,
                    OrderNumber = $"ORD-{o.Id:D6}",
                    TotalAmount = o.TotalAmount,
                    Status = o.StatusText ?? o.Status.ToString(),
                    OrderDate = o.CreatedAt ?? DateTime.Now
                }).ToList()
            };

            return View(viewModel);
        }

        [HttpGet]
        [Route("Edit/{id}")]
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                SetErrorMessage("Kullanıcı bulunamadı!");
                return RedirectToAction("Index");
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var allRoles = _roleManager.Roles.Select(r => r.Name).ToList();

            var viewModel = new EditUserViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Balance = user.Balance,
                AvailableRoles = allRoles,
                SelectedRoles = userRoles.ToList(),
                LockoutEnabled = user.LockoutEnabled
            };

            return View(viewModel);
        }

        [HttpPost]
        [Route("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var allRoles = _roleManager.Roles.Select(r => r.Name).ToList();
                model.AvailableRoles = allRoles;
                return View(model);
            }

            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                SetErrorMessage("Kullanıcı bulunamadı!");
                return RedirectToAction("Index");
            }

            // Kullanıcı bilgilerini güncelle
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.PhoneNumber = model.PhoneNumber;
            user.Balance = model.Balance;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                foreach (var error in updateResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(model);
            }

            // Rolleri güncelle
            var currentRoles = await _userManager.GetRolesAsync(user);
            var rolesToRemove = currentRoles.Except(model.SelectedRoles).ToList();
            var rolesToAdd = model.SelectedRoles.Except(currentRoles).ToList();

            if (rolesToRemove.Any())
            {
                await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
            }

            if (rolesToAdd.Any())
            {
                await _userManager.AddToRolesAsync(user, rolesToAdd);
            }

            SetSuccessMessage("Kullanıcı başarıyla güncellendi!");
            return RedirectToAction("Detail", new { id = model.Id });
        }

        [HttpPost]
        [Route("LockUser/{id}")]
        public async Task<IActionResult> LockUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                SetErrorMessage("Kullanıcı bulunamadı!");
                return RedirectToAction("Index");
            }

            // 100 yıl kilitle (sonsuz)
            await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddYears(100));
            SetSuccessMessage("Kullanıcı hesabı kilitlendi!");

            return RedirectToAction("Detail", new { id });
        }

        [HttpPost]
        [Route("UnlockUser/{id}")]
        public async Task<IActionResult> UnlockUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                SetErrorMessage("Kullanıcı bulunamadı!");
                return RedirectToAction("Index");
            }

            await _userManager.SetLockoutEndDateAsync(user, null);
            await _userManager.ResetAccessFailedCountAsync(user);
            SetSuccessMessage("Kullanıcı hesabı kilidi açıldı!");

            return RedirectToAction("Detail", new { id });
        }

        [HttpPost]
        [Route("Delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                SetErrorMessage("Kullanıcı bulunamadı!");
                return RedirectToAction("Index");
            }

            // Admin kullanıcıyı silmeyi engelle
            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Contains("Admin"))
            {
                SetErrorMessage("Admin kullanıcıları silinemez!");
                return RedirectToAction("Index");
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                SetSuccessMessage("Kullanıcı başarıyla silindi!");
            }
            else
            {
                SetErrorMessage("Kullanıcı silinirken bir hata oluştu!");
            }

            return RedirectToAction("Index");
        }
    }
}
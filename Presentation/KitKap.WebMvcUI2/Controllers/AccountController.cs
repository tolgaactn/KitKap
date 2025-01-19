using Kitkap.Entity.Services;
using Kitkap.Entity.ViewModels.CategoryViewModels;
using Kitkap.Entity.ViewModels.UserViewModels;
using KitKap.Service.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kitkap.WebMvcUI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _accountService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpGet("id")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _accountService.FindById(id);
            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(RegisterUserViewModel model)
        {
            await _accountService.CreateUserAsync(model);
            return Ok(model.UserName);
        }
        [HttpPut]
        public async Task<IActionResult> UpdateUser(UpdateUserViewModel model)
        {
            await _accountService.UpdateUserAsync(model);
            return Ok(model.UserName + "kullanıcısının bilgileri güncellendi");
        }
        [HttpDelete]
        public async Task<IActionResult> RemoveUser(DeactivateUserViewModel model)
        {
            await _accountService.DeactivateUserAsync(model);
            return Ok(model.Id + "kullanıcısı silindi");
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserViewModel model)
        {
            var token = await _accountService.LoginAsync(model);
            return Ok(new { Token = token });
        }

    }
}

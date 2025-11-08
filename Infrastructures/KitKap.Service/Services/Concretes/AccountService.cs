using AutoMapper;
using Kitkap.Entity.Services;
using Kitkap.Service.Dtos.AddressDtos;
using Kitkap.Service.Dtos.UserDtos;
using KitKap.DataAccess.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
namespace KitKap.Service.Services.Concretes
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AccountService(
            UserManager<AppUser> userManager,
            IMapper mapper,
            SignInManager<AppUser> signInManager,
            IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _mapper = mapper;
            _signInManager = signInManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<string> CreateUserAsync(RegisterUserDto model)
        {
            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                return "Bu email adresi zaten kayıtlı.";
            }

            AppUser user = new AppUser
            {
                UserName = model.UserName,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Balance = 0,
                IsActived = true
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "BireyselMusteri");
                return "OK";
            }

            return string.Join(", ", result.Errors.Select(e => e.Description));
        }

        public async Task<AuthResponse> LoginAsync(LoginUserDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                return new AuthResponse
                {
                    IsSuccessful = false,
                    Errors = new List<string> { "Email veya şifre hatalı" }
                };
            }

            if (!user.IsActived)
            {
                return new AuthResponse
                {
                    IsSuccessful = false,
                    Errors = new List<string> { "Hesabınız pasif durumdadır." }
                };
            }

            // Cookie Authentication ile giriş yap
            var result = await _signInManager.PasswordSignInAsync(
                user,
                model.Password,
                isPersistent: model.RememberMe, // Remember Me
                lockoutOnFailure: true // 3 yanlış denemeden sonra kilitle
            );

            if (result.Succeeded)
            {
                // Login history kaydet (opsiyonel)
                await SaveLoginHistoryAsync(user.Id, true, model.IpAddress);

                return new AuthResponse
                {
                    IsSuccessful = true,
                    UserName = user.UserName
                };
            }

            if (result.IsLockedOut)
            {
                return new AuthResponse
                {
                    IsSuccessful = false,
                    Errors = new List<string> { "Hesabınız kilitlendi. Lütfen daha sonra tekrar deneyin." }
                };
            }

            // Başarısız giriş
            await SaveLoginHistoryAsync(user.Id, false, model.IpAddress);

            return new AuthResponse
            {
                IsSuccessful = false,
                Errors = new List<string> { "Email veya şifre hatalı" }
            };
        }

        private async Task SaveLoginHistoryAsync(string userId, bool isSuccessful, string ipAddress)
        {
            // Eğer LoginHistory kullanmak isterseniz:
            // var history = new LoginHistory
            // {
            //     UserId = userId,
            //     LoginDate = DateTime.Now,
            //     IpAddress = ipAddress ?? "Unknown",
            //     IsSuccessful = isSuccessful
            // };
            // await _dbContext.LoginHistories.AddAsync(history);
            // await _dbContext.SaveChangesAsync();

            // Şimdilik boş bırakıyoruz
            await Task.CompletedTask;
        }

        public async Task<GetByIdUserDto> FindById(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                throw new Exception("Kullanıcı bulunamadı.");

            return _mapper.Map<GetByIdUserDto>(user);
        }

        public async Task<List<RequestUserDto>> GetAllUsersAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            return _mapper.Map<List<RequestUserDto>>(users);
        }

        public async Task DeactivateUserAsync(DeactivateUserDto model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
                throw new Exception("Kullanıcı bulunamadı.");

            user.IsActived = false;
            await _userManager.UpdateAsync(user);
        }

        public async Task UpdateUserAsync(UpdateUserDto model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
                throw new Exception("Kullanıcı bulunamadı.");

            user.UserName = model.UserName;
            user.Email = model.Email;
            user.PhoneNumber = model.PhoneNumber;
            user.Balance = model.Balance;
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.AddressId = model.AddressId;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"Güncelleme başarısız: {errors}");
            }
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<IList<string>> GetRolesAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return new List<string>();

            return await _userManager.GetRolesAsync(user);
        }

        public async Task<RequestUserDto?> GetUserByEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
             if (user == null)
                return null;

             return _mapper.Map<RequestUserDto>(user);
        }

        public async Task<ProfileDto?> GetUserProfileAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return null;

            return _mapper.Map<ProfileDto>(user);
        }

        public async Task<bool> UpdateUserProfileAsync(UpdateProfileDto model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
                return false;

            // Email ve UserName değişikliği için kontrol
            if (user.UserName != model.UserName)
            {
                var existingUserName = await _userManager.FindByNameAsync(model.UserName);
                if (existingUserName != null && existingUserName.Id != user.Id)
                {
                    throw new Exception("Bu kullanıcı adı zaten kullanılıyor.");
                }
            }

            // Bilgileri güncelle
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.PhoneNumber = model.PhoneNumber;
            user.UserName = model.UserName;

            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> ChangePasswordAsync(ChangePasswordDto model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
                return false;

            var result = await _userManager.ChangePasswordAsync(
                user,
                model.CurrentPassword,
                model.NewPassword
            );

            return result.Succeeded;
        }
    }

}

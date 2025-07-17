using AutoMapper;
using Kitkap.Entity.Services;
using Kitkap.Service.Dtos.AddressDtos;
using KitKap.DataAccess.Identity;
using KitKap.Service.Jwt;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace KitKap.Service.Services.Concretes
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;
        private readonly JwtSettings _jwtSettings;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountService(UserManager<AppUser> userManager, IMapper mapper, IOptions<JwtSettings> jwtSettings, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _mapper = mapper;
            _jwtSettings = jwtSettings.Value;
            _signInManager = signInManager;
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
                //PhoneNumber = model.PhoneNumber,
                Balance = 0,
                IsActived = true
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // Varsayılan rol ata
                await _userManager.AddToRoleAsync(user, "BireyselMusteri");
                return "OK";
            }

            return string.Join(", ", result.Errors.Select(e => e.Description));
        }

        public async Task<AuthResponse> LoginAsync(LoginUserDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return new AuthResponse { IsSuccessful = false, Errors = new List<string> { "Email bulunamadı" } };

            var passwordValid = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!passwordValid)
                return new AuthResponse { IsSuccessful = false, Errors = new List<string> { "Şifre yanlış" } };


            if (!user.IsActived)
            {
                throw new Exception("Hesabınız pasif durumdadır.");
            }

            var roles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.UserName)
        };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddMinutes(_jwtSettings.ExpiresInMinutes);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return new AuthResponse
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = expires,
                UserName = user.UserName,
                IsSuccessful = true
            };
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
            return await _userManager.GetRolesAsync(user);
        }
    }

}

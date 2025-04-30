using AutoMapper;
using Kitkap.Entity.Services;
using Kitkap.Service.Dtos.AddressDtos;
using Kitkap.Service.Dtos.UserDtos;
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
        private readonly IConfiguration _configuration;
        private readonly JwtSettings _jwtSettings;

        public AccountService(UserManager<AppUser> userManager, IMapper mapper, IConfiguration configuration, IOptions<JwtSettings> jwtSettings)
        {
            _userManager = userManager;
            _mapper = mapper;
            _configuration = configuration;
            _jwtSettings = jwtSettings.Value;
        }

        public async Task<string> CreateUserAsync(RegisterUserDto model)
        {
            string message = string.Empty;
            AppUser user = new AppUser()
            {

                UserName = model.UserName,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Balance = 0,
                PhoneNumber = model.PhoneNumber
            };
            var identityResult = await _userManager.CreateAsync(user, model.ConfirmPassword);

            if (identityResult.Succeeded)
            {
                message = "OK";
            }
            else
            {
                foreach (var error in identityResult.Errors)
                {
                    message = error.Description;
                }
            }
            return message;
        }

        public async Task<GetByIdUserDto> FindById(string id)
        {

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                throw new Exception("Kullanıcı bulunamadı");
            }
            return _mapper.Map<GetByIdUserDto>(user);
        }

        public async Task<List<RequestUserDto>> GetAllUsersAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            return _mapper.Map<List<RequestUserDto>>(users);
        }

        public async Task<AuthResponse> LoginAsync(LoginUserDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                throw new Exception("Giriş başarısız");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtSettings.Secret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email)
        }),
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiresInMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return new AuthResponse
            {
                Token = tokenString,
                Expiration = tokenDescriptor.Expires ?? DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiresInMinutes),
                UserName = user.UserName
            };
        }

        public async Task DeactivateUserAsync(DeactivateUserDto model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);

            if (user == null)
            {
                throw new Exception("Bu id'de kişi tanımlı değil");
            }
            user.IsActived = false;
            await _userManager.UpdateAsync(user);

        }

        public async Task UpdateUserAsync(UpdateUserDto model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                throw new Exception("Bu id'de kişi tanımlı değil");
            }
            user.UserName = model.UserName;
            user.AddressId = model.AddressId;
            user.Email = model.Email;
            user.Balance = model.Balance;
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.PhoneNumber = model.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"Kullanıcı güncellenemedi. Hatalar: {errors}");
            }
        }


    }
}

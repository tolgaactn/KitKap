using KitKap.DataAccess.Identity;
using KitKap.DataAccess.Repositories;
using KitKap.DataAccess.UnitOfWorks;
using Kitkap.Entity.Repositories;
using Kitkap.Entity.Services;
using Kitkap.Entity.UnitOfWorks;
using KitKap.Service.Mapping;
using KitKap.Service.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KitKap.DataAccess.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using KitKap.Service.Jwt;
using Microsoft.AspNetCore.Identity;
using KitKap.Service.Services.Concretes;
using KitKap.Service.Services.Interfaces;
using Kitkap.Service.Services;

namespace KitKap.Service.Extensions
{
    public static class DependencyExtensions
    {
        public static void AddExtensions(this IServiceCollection services,IConfiguration configuration)
        {
            services.AddIdentityCore<AppUser>(
    opt =>
    {
        opt.Password.RequireNonAlphanumeric = false;
        opt.Password.RequiredLength = 3;
        opt.Password.RequireLowercase = false;
        opt.Password.RequireUppercase = false;
        opt.Password.RequireDigit = false;
        opt.User.RequireUniqueEmail = true;
        opt.Lockout.MaxFailedAccessAttempts = 3;
        opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1);
    }
).AddEntityFrameworkStores<KitKapDbContext>();

            services.ConfigureApplicationCookie(opt =>
            {
                opt.LoginPath = new PathString("/Account/Login");
                opt.LogoutPath = new PathString("/Account/Logout");
                //opt.AccessDeniedPath = new PathString("/Account/AccessDenied");
                opt.ExpireTimeSpan = TimeSpan.FromMinutes(10);
                opt.SlidingExpiration = true; //10 dk. dolmadan kullanıcı yeniden login olursa süre baştan başlar.

                opt.Cookie = new CookieBuilder()
                {
                    Name = "Identity.App.Cookie",
                    HttpOnly = true
                };
            });

            var jwtSettings = configuration.GetSection("JwtSettings");
            services.Configure<JwtSettings>(jwtSettings);

            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(opt =>
            {
                var key = Encoding.UTF8.GetBytes(jwtSettings["Secret"]);
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
            });

            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IAddressService, AddressService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<ITransactionService, TransactionService>();
            services.AddScoped<IAboutService, AboutService>();
            services.AddScoped<IProductImageService, ProductImageService>();
            services.AddScoped<IFileService, FileService>();
           // services.AddScoped(typeof(IAccountService), typeof(AccountService));

            services.AddAutoMapper(typeof(MappingProfile));
        }
    }
}

using Kitkap.Entity.Repositories;
using Kitkap.Entity.Services;
using Kitkap.Entity.UnitOfWorks;
using Kitkap.Service.Services;
using KitKap.DataAccess.Contexts;
using KitKap.DataAccess.Identity;
using KitKap.DataAccess.Repositories;
using KitKap.DataAccess.UnitOfWorks;
using KitKap.Service.Mapping;
using KitKap.Service.Services;
using KitKap.Service.Services.Concretes;
using KitKap.Service.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace KitKap.Service.Extensions
{
    public static class DependencyExtensions
    {
        public static void AddExtensions(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment env)
        {
            // ========== IDENTITY CONFIGURATION ==========
            services.AddIdentity<AppUser, IdentityRole>(options =>
            {
                // Password Policy
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 3;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireDigit = false;

                // User Policy
                options.User.RequireUniqueEmail = true;

                // Lockout Policy
                options.Lockout.MaxFailedAccessAttempts = 3;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1);
            })
            .AddEntityFrameworkStores<KitKapDbContext>()
            .AddDefaultTokenProviders();

            // ========== COOKIE AUTHENTICATION CONFIGURATION ==========
            services.ConfigureApplicationCookie(options =>
            {
                // Path Configuration
                options.LoginPath = new PathString("/Account/Login");
                options.LogoutPath = new PathString("/Account/Logout");
                options.AccessDeniedPath = new PathString("/Account/AccessDenied");

                // Session Timeout
                options.ExpireTimeSpan = TimeSpan.FromMinutes(60); // 1 saat oturum
                options.SlidingExpiration = true; // Kullanıcı aktifse süre yenilenir

                // Cookie Configuration
                options.Cookie = new CookieBuilder()
                {
                    Name = "KitKap.Auth.Cookie", // Daha açıklayıcı isim
                    HttpOnly = true, // XSS koruması
                    SameSite = env.IsDevelopment() ? SameSiteMode.Lax : SameSiteMode.Strict,
                    SecurePolicy = env.IsDevelopment() ? CookieSecurePolicy.None : CookieSecurePolicy.Always,
                    IsEssential = true // GDPR için gerekli
                };
            });

            // ========== DEPENDENCY INJECTION ==========

            // Identity Services
            services.AddScoped<RoleManager<IdentityRole>>();

            // Repository Pattern
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Business Services
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IAddressService, AddressService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<ITransactionService, TransactionService>();
            services.AddScoped<IAboutService, AboutService>();
            services.AddScoped<IProductImageService, ProductImageService>();
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IShoppingCartService, ShoppingCartService>();
            services.AddScoped<IOrderService, OrderService>();

            // AutoMapper
            services.AddAutoMapper(typeof(MappingProfile));

            // HttpContextAccessor (LoginHistory için gerekli)
            services.AddHttpContextAccessor();
        }
    }
}

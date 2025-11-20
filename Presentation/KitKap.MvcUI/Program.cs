using KitKap.DataAccess.Contexts;
using Microsoft.EntityFrameworkCore;
using KitKap.Service.Extensions;
using KitKap.MvcUI.SeedData;
using KitKap.DataAccess.Identity;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<KitKapDbContext>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("ConnStr"))
);

builder.Services.AddExtensions(builder.Configuration, builder.Environment);
builder.Services.AddSession();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();

// OTOMATIK MIGRATION VE ADMIN OLUŞTURMA
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<KitKapDbContext>();
        context.Database.Migrate(); // Otomatik migration çalıştır

        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogInformation("Migration başarıyla tamamlandı");

        // Role seeding migration'dan sonra çalışsın
        await RoleSeeder.SeedRolesAsync(services);

        // ADMIN KULLANICI OLUŞTUR
        var userManager = services.GetRequiredService<UserManager<AppUser>>();
        var adminEmail = "admin@kitkap.com";
        var adminPassword = "Admin123!"; // ÖNEMLİ: Güçlü şifre kullan!

        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            adminUser = new AppUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                FirstName = "Tolga",
                LastName = "Çetin",
                IsActived = true,
            };

            var result = await userManager.CreateAsync(adminUser, adminPassword);

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
                logger.LogInformation("✅ Admin kullanıcı oluşturuldu: {Email}", adminEmail);
            }
            else
            {
                logger.LogError("❌ Admin oluşturulamadı: {Errors}",
                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
        else
        {
            logger.LogInformation("Admin kullanıcı zaten mevcut: {Email}", adminEmail);
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Migration veya seeding sırasında hata: {Message}", ex.Message);
        throw; // Hata varsa uygulama başlamasın
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

// Admin Area Route
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
);

// Default Route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();
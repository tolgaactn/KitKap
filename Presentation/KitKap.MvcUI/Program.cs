using KitKap.DataAccess.Contexts;
using Microsoft.EntityFrameworkCore;
using KitKap.Service.Extensions;
using KitKap.MvcUI.SeedData;
using KitKap.DataAccess.Identity;

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

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await RoleSeeder.SeedRolesAsync(services);
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
using KitKap.DataAccess.Contexts;
using Microsoft.EntityFrameworkCore;
using KitKap.Service.Extensions;
using KitKap.MvcUI.SeedData; // Doðru namespace

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<KitKapDbContext>(
        options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("ConnStr")
    ));

builder.Services.AddExtensions(builder.Configuration, builder.Environment);
builder.Services.AddSession();

builder.Logging.ClearProviders();      // Default log saðlayýcýlarý kaldýrýlýr
builder.Logging.AddConsole();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await RoleSeeder.SeedRolesAsync(services); // Rol seed iþlemi
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

//app.MapControllerRoute(
//      name: "areas",
//      pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
//    );


//app.MapControllerRoute(
//      name: "area",
//      pattern: "{controller=Product}/{action=Index}/{area=Admin}/{id?}"
//    );


//app.MapControllerRoute(
//    name: "default",
//    pattern: "{area=Admin}/{controller=Product}/{action=Index}/{id?}");


app.Run();

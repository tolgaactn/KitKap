using Kitkap.Entity.Services;
using Kitkap.Service.Services;
using KitKap.DataAccess.Identity;
using KitKap.MvcUI.Areas.Admin.ViewModels.DashboardViewModels;
using KitKap.Service.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static Kitkap.Entity.Entities.Order;

namespace KitKap.MvcUI.Areas.Admin.Controllers
{
    [Route("Admin")]
    public class AdminHomeController : BaseAdminController
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IOrderService _orderService;
        private readonly UserManager<AppUser> _userManager;

        public AdminHomeController(
            IProductService productService,
            ICategoryService categoryService,
            IOrderService orderService,
            UserManager<AppUser> userManager)
        {
            _productService = productService;
            _categoryService = categoryService;
            _orderService = orderService;
            _userManager = userManager;
        }

        [Route("")]
        [Route("Dashboard")]
        public async Task<IActionResult> Index()
        {
            var viewModel = new DashboardViewModel();

            // 1. Tüm ürünleri çek
            var allProducts = (await _productService.GetAllProducts())
                .Where(p => !p.IsDeleted)
                .ToList();

            viewModel.TotalProducts = allProducts.Count;

            // 2. Kategorileri çek
            var allCategories = (await _categoryService.GetAllCategories())
                .Where(c => !c.IsDeleted)
                .ToList();

            viewModel.TotalCategories = allCategories.Count;

            // 3. Kullanıcı sayısını al
            viewModel.TotalUsers = _userManager.Users.Count();

            // 4. Stok uyarıları
            viewModel.LowStockProducts = allProducts.Count(p => p.Stock > 0 && p.Stock <= 5);
            viewModel.OutOfStockProducts = allProducts.Count(p => p.Stock == 0);

            // 5. Son eklenen ürünler (5 adet)
            viewModel.RecentProducts = allProducts
                .OrderByDescending(p => p.CreatedAt)
                .Take(5)
                .Select(p => new RecentProductViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    Stock = p.Stock,
                    ImageUrl = p.ProductImages?.FirstOrDefault(img => img.IsMain && !img.IsDeleted)?.ImageUrl
                        ?? "/template/assets/images/default-product.png",
                    CreatedDate = p.CreatedAt
                })
                .ToList();

            // 6. Sipariş istatistikleri - Tüm kullanıcıların siparişlerini topla
            try
            {
                var allUsers = _userManager.Users.ToList();
                int totalOrders = 0;
                int pendingOrders = 0;
                int processingOrders = 0;
                int completedOrders = 0;
                int cancelledOrders = 0;
                decimal totalRevenue = 0;
                decimal monthlyRevenue = 0;
                var recentOrdersList = new List<RecentOrderViewModel>();

                foreach (var user in allUsers)
                {
                    var userOrders = await _orderService.GetOrdersByUserAsync(user.Id);

                    totalOrders += userOrders.Count();
                    pendingOrders += userOrders.Count(o => o.Status == OrderStatus.Pending);
                    processingOrders += userOrders.Count(o => o.Status == OrderStatus.Processing);
                    completedOrders += userOrders.Count(o => o.Status == OrderStatus.Delivered);
                    cancelledOrders += userOrders.Count(o => o.Status == OrderStatus.Cancelled);

                    totalRevenue += userOrders
                        .Where(o => o.Status == OrderStatus.Delivered)
                        .Sum(o => o.TotalAmount);

                    monthlyRevenue += userOrders
                        .Where(o => o.Status == OrderStatus.Delivered
                            && o.CreatedAt.HasValue
                            && o.CreatedAt.Value >= DateTime.Now.AddMonths(-1))
                        .Sum(o => o.TotalAmount);

                    // Son 5 sipariş için
                    var userRecentOrders = userOrders
                        .OrderByDescending(o => o.CreatedAt)
                        .Take(5)
                        .Select(o => new RecentOrderViewModel
                        {
                            Id = o.Id,
                            CustomerName = $"{user.FirstName} {user.LastName}",
                            TotalAmount = o.TotalAmount,
                            Status = o.StatusText ?? o.Status.ToString(),
                            OrderDate = o.CreatedAt ?? DateTime.Now
                        });

                    recentOrdersList.AddRange(userRecentOrders);
                }

                viewModel.TotalOrders = totalOrders;
                viewModel.PendingOrders = pendingOrders;
                viewModel.ProcessingOrders = processingOrders;
                viewModel.CompletedOrders = completedOrders;
                viewModel.CancelledOrders = cancelledOrders;
                viewModel.TotalRevenue = totalRevenue;
                viewModel.MonthlyRevenue = monthlyRevenue;

                // Son siparişleri tarihe göre sırala ve ilk 5'ini al
                viewModel.RecentOrders = recentOrdersList
                    .OrderByDescending(o => o.OrderDate)
                    .Take(5)
                    .ToList();
            }
            catch (Exception ex)
            {
                // Hata durumunda default değerler kalacak
                SetWarningMessage("Sipariş bilgileri yüklenirken bir hata oluştu.");
            }

            return View(viewModel);
        }
    }
}
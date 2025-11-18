using KitKap.MvcUI.Areas.Admin.ViewModels.DashboardViewModels;

public class DashboardViewModel
{
    // Genel İstatistikler
    public int TotalProducts { get; set; }
    public int TotalCategories { get; set; }
    public int TotalOrders { get; set; }
    public int TotalUsers { get; set; }

    // Sipariş İstatistikleri
    public int PendingOrders { get; set; }
    public int ProcessingOrders { get; set; }
    public int CompletedOrders { get; set; }
    public int CancelledOrders { get; set; }

    // Finansal
    public decimal TotalRevenue { get; set; }
    public decimal MonthlyRevenue { get; set; }

    // Stok Uyarıları
    public int LowStockProducts { get; set; }
    public int OutOfStockProducts { get; set; }

    // Son Aktiviteler
    public List<RecentProductViewModel> RecentProducts { get; set; } = new();
    public List<RecentOrderViewModel> RecentOrders { get; set; } = new();
}
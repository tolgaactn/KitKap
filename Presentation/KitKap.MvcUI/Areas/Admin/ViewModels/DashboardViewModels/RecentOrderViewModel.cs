namespace KitKap.MvcUI.Areas.Admin.ViewModels.DashboardViewModels
{
    public class RecentOrderViewModel
    {
        public long Id { get; set; }
        public string CustomerName { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public DateTime OrderDate { get; set; }
    }
}

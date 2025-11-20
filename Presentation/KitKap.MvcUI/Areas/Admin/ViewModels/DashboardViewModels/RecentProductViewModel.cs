namespace KitKap.MvcUI.Areas.Admin.ViewModels.DashboardViewModels
{
    public class RecentProductViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string ImageUrl { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}

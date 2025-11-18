namespace KitKap.MvcUI.ViewModels.OrderViewModels
{
    public class UpdateOrderStatusViewModel
    {
        public int OrderId { get; set; }
        public string NewStatus { get; set; }
        public string? TrackingNumber { get; set; }
        public string? CargoCompany { get; set; }
        public string? AdminNote { get; set; }
    }
}

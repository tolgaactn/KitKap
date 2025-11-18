namespace KitKap.MvcUI.Areas.Admin.ViewModels.UserViewModels
{
    public class UserOrderSummaryViewModel
    {
        public int OrderId { get; set; }
        public string OrderNumber { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public DateTime OrderDate { get; set; }
    }
}

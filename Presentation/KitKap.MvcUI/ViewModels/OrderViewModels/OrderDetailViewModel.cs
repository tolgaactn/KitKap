namespace KitKap.MvcUI.ViewModels.OrderViewModels
{
    public class OrderDetailViewModel
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; }
        public string BuyerName { get; set; }
        public string BuyerEmail { get; set; }
        public string BuyerPhone { get; set; }
        public string ShippingAddress { get; set; }
        public string Status { get; set; }
        public string StatusText { get; set; }
        public string PaymentMethod { get; set; }
        public List<OrderItemViewModel> Items { get; set; } = new();
        public decimal SubTotal { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? ShippedAt { get; set; }
        public DateTime? DeliveredAt { get; set; }
        public string? TrackingNumber { get; set; }
        public string? CargoCompany { get; set; }
        public string? CustomerNote { get; set; }
        public string? AdminNote { get; set; }
    }
}

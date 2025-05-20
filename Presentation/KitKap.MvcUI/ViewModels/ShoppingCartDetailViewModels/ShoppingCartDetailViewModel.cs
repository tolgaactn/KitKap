using Kitkap.Entity.Entities;

namespace KitKap.MvcUI.ViewModels.ShoppingCartDetailViewModels
{
    public class ShoppingCartDetailViewModel
    {
        public int productId { get; set; }
        public string productName { get; set; }
        public int productQuantity { get; set; }
        public decimal productPrice { get; set; }
        public List<string> ImageUrls { get; set; }
    }
}

using Kitkap.Entity.Entities;

namespace KitKap.MvcUI.ViewModels.ShoppingCartViewModels
{
    public class ShoppingCartItemViewModel
    {
        public int Id { get; set; }
        public int ShoppingCartId { get; set; }  // FK
        public long ProductId { get; set; }       // FK

        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }   // O anki ürün fiyatı snapshot


        public string ProductName { get; set; }
        public string ImageUrl { get; set; }

        public decimal TotalPrice => Quantity * UnitPrice;
    }
}

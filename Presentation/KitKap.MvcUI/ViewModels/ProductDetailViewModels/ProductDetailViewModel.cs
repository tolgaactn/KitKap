using Kitkap.Entity.Entities;

namespace KitKap.MvcUI.ViewModels.ProductDetailViewModels
{
    public class ProductDetailViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string CategoryName { get; set; }
        public string ImageUrl { get; set; }
        public string OwnerUserName { get; set; }


        public Category Category { get; set; }
    }
}

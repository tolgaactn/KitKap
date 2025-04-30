using Kitkap.Entity.Entities;

namespace KitKap.MvcUI.ViewModels.ProductListViewModels
{
    public class ProductListViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string CategoryName { get; set; }
        public decimal Price { get; set; }
        public Category Category { get; set; }
        public string ImageUrl  { get; set; }
        
    }
}

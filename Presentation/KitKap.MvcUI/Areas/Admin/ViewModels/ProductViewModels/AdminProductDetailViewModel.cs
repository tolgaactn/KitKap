using Kitkap.Entity.Entities;

namespace KitKap.MvcUI.Areas.Admin.ViewModels.ProductViewModels
{
    public class AdminProductDetailViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string CategoryName { get; set; }
        public string ImageUrl { get; set; }
        public List<string> ImageUrls { get; set; }

        public Category Category { get; set; }
    }
}

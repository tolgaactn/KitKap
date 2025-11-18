namespace KitKap.MvcUI.ViewModels.HomeViewModels
{
    public class CategorySlideViewModel
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int ProductCount { get; set; }
        public string ImageUrl { get; set; }
        public ProductCardViewModel FeaturedProduct { get; set; }
    }
}

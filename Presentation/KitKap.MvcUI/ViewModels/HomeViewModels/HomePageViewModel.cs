namespace KitKap.MvcUI.ViewModels.HomeViewModels
{
    public class HomePageViewModel
    {
        public List<CategorySlideViewModel> CategorySlides { get; set; } = new();
        public List<ProductCardViewModel> BestSellers { get; set; } = new();
        public List<ProductCardViewModel> FeaturedBooks { get; set; } = new();
        public List<ProductCardViewModel> RecentBooks { get; set; } = new();
    }
}

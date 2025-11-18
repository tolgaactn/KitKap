namespace KitKap.MvcUI.ViewModels.HomeViewModels
{
    public class ProductCardViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public decimal? OldPrice { get; set; }
        public string ImageUrl { get; set; }
        public string CategoryName { get; set; }
        public bool IsHot { get; set; }
        public int? DiscountPercentage { get; set; }
    }
}

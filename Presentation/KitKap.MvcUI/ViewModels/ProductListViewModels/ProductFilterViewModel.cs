namespace KitKap.MvcUI.ViewModels.ProductListViewModels
{
    public class ProductFilterViewModel
    {
        // Filtreleme Parametreleri
        public int? CategoryId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? Condition { get; set; } // "New", "Used"
        public string? Author { get; set; }
        public string? Publisher { get; set; }
        public bool? InStockOnly { get; set; } = false;

        // Sıralama
        public string? SortBy { get; set; } = "recommended"; // recommended, price-asc, price-desc, newest, bestseller

        // Sayfalama
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 12;

        // Arama
        public string? SearchQuery { get; set; }
    }
}
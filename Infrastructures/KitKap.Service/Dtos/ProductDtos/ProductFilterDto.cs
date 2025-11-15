namespace KitKap.Service.Dtos.ProductDtos
{
    public class ProductFilterDto
    {
        // Filtreleme
        public int? CategoryId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? Condition { get; set; } // "New", "Used", "All"
        public bool? InStockOnly { get; set; } = false;
        public string? SearchQuery { get; set; }

        // Sıralama
        public string? SortBy { get; set; } = "newest";
        // Değerler: "recommended", "price-asc", "price-desc", "newest", "name-asc", "name-desc"

        // Sayfalama
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 12;
    }
}
namespace KitKap.MvcUI.ViewModels.ProductListViewModels
{
    public class ProductListPageViewModel
    {
        // Ürünler
        public List<ProductListViewModel> Products { get; set; } = new();

        // Filtre Parametreleri
        public ProductFilterViewModel Filters { get; set; } = new();

        // Kategoriler (Filtre için)
        public List<CategoryViewModel> Categories { get; set; } = new();

        // Sonuç Bilgileri
        public int TotalProducts { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }

        // Fiyat Aralığı (Tüm ürünlerin min-max'ı)
        public decimal MinAvailablePrice { get; set; }
        public decimal MaxAvailablePrice { get; set; }

        // Aktif Filtreler (UI'da göstermek için)
        public List<ActiveFilterViewModel> ActiveFilters { get; set; } = new();
    }

    public class ActiveFilterViewModel
    {
        public string Label { get; set; }
        public string Value { get; set; }
        public string RemoveUrl { get; set; }
    }

    public class CategoryViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ProductCount { get; set; }
    }
}
using Kitkap.Entity.Entities;
using Kitkap.Service.Dtos.AddressDtos;

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
        public List<string> ImageUrls { get; set; }
        public string OwnerUserName { get; set; }

        // Book özel alanları
        public string? Author { get; set; }
        public int? ISBN { get; set; }
        public DateTime? PublicationDate { get; set; }
        public string? Language { get; set; }
        public string? Condition { get; set; }

        public ProductPreviewViewModel? PreviousProduct { get; set; }
        public ProductPreviewViewModel? NextProduct { get; set; }

        public bool IsLastProduct { get; set; }

        public Category Category { get; set; }
    }  
}

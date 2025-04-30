using Kitkap.Entity.Entities;
using KitKap.MvcUI.Areas.Admin.ViewModels.ProductImagesViewModels;
using static Kitkap.Entity.Entities.Product;

namespace KitKap.MvcUI.Areas.Admin.ViewModels.ProductViewModels
{
    using Microsoft.AspNetCore.Http;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class EditProductViewModel
    {
        public long Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public int Stock { get; set; }

        [Required]
        public int CategoryId { get; set; }
        public ProductStatus Status { get; set; }
        public bool IsDeleted { get; set; }
        public string OwnerId { get; set; }

        // Mevcut ürün görselleri
        public List<ExistingProductImageViewModel>? ExistingImages { get; set; }

        // Yeni yüklenecek görseller
        public List<IFormFile>? NewProductImages { get; set; }

        // Seçilen yeni ana görsel ID'si
        public long? SelectedMainImageId { get; set; }

        // Silinecek görsellerin ID'leri
        public List<long> ImagesToDelete { get; set; } = new();
    }
}

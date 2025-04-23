using Kitkap.Entity.Entities;
using static Kitkap.Entity.Entities.Product;

namespace KitKap.MvcUI.Areas.Admin.ViewModels.ProductViewModels
{
    public class GetAllProductViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string CategoryName { get; set; } // 👈 yeni eklenen alan
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string OwnerId { get; set; }
        public int CategoryId { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public ProductStatus Status { get; set; }
        public Category Category { get; set; }
        public string ImageUrl { get; set; }
    }
}

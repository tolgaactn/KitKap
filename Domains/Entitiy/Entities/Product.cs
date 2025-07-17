using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kitkap.Entity.Entities
{
	public class Product
	{
        public long Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
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
		public ICollection<ProductImage> ProductImages { get; set; }
        public enum ProductStatus
        {
            OutOfStock = 0,
            InStock = 1,
            Discontinued = 2
        }        
    }
}

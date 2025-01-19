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
		public bool IsAvailable { get; set; }
		public string OwnerId { get; set; }
		public int CategoryId { get; set; }
		public bool IsDeleted { get; set; } = false;

		public Category Category { get; set; }
		public ICollection<ProductImage> ProductImages { get; set; }

    }
}

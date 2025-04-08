using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kitkap.Entity.Entities
{
	public class ProductImage
	{
		public long Id { get; set; }
		public string ImageUrl { get; set; }
		public long ProductId { get; set; }
        public string? AltText { get; set; }
        public bool IsMain { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; } = false;
        public Product Product { get; set; }
	}
}

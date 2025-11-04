using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kitkap.Entity.Entities
{
    public class ShoppingCart
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public string? GuestId { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsCheckedOut { get; set; } = false;

        public List<ShoppingCartItem> Items { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KitKap.Service.Dtos.ShoppingCartDtos
{
    public class ShoppingCartDto
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public string? GuestId { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsCheckedOut { get; set; }
        public List<ShoppingCartItemDto> Items { get; set; }
        public decimal TotalPrice => Items.Sum(x => x.Quantity * x.UnitPrice);
        public int TotalItemCount => Items?.Sum(i => i.Quantity) ?? 0;
    }
}

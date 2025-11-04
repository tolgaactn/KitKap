using Kitkap.Entity.Entities;
using KitKap.Service.Dtos.ShoppingCartDtos;

namespace KitKap.MvcUI.ViewModels.ShoppingCartViewModels
{
    public class ShoppingCartViewModel
    {
        public int Id { get; set; }

        // Hem giriş yapan hem misafir kullanıcıyı desteklemek için
        public string? UserId { get; set; }
        public string? GuestId { get; set; }

        public DateTime CreatedAt { get; set; }
        public bool IsCheckedOut { get; set; } = false;

        public List<ShoppingCartItemViewModel> Items { get; set; }
        public decimal TotalPrice => Items.Sum(x => x.Quantity * x.UnitPrice);
        public int TotalItemCount => Items?.Sum(i => i.Quantity) ?? 0;
    }
}

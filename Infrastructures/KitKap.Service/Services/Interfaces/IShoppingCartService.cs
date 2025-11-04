using KitKap.Service.Dtos.ShoppingCartDtos;

namespace KitKap.Service.Services.Interfaces
{
    public interface IShoppingCartService
    {
        // Sepete ürün ekle
        Task AddToCartAsync(string? userId, string? guestId, AddCartItemDto dto);

        // Sepeti getir
        Task<ShoppingCartDto> GetCartAsync(string? userId, string? guestId);

        // Sepetten ürün sil
        Task RemoveFromCartAsync(string? userId, string? guestId, long productId);

        // Ürün miktarını güncelle
        Task UpdateQuantityAsync(string? userId, string? guestId, long productId, int newQuantity);

        // Toplam fiyatı hesapla
        Task<decimal> GetTotalPriceAsync(string? userId, string? guestId);

        // Sepeti tamamen temizle
        Task ClearCartAsync(string? userId, string? guestId);

        // Misafir sepetini kullanıcı sepetine birleştir (login olunca)
        Task MergeGuestCartToUserAsync(string userId, string guestId);
    }
}
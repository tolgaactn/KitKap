using AutoMapper;
using Kitkap.Entity.Entities;
using Kitkap.Entity.UnitOfWorks;
using KitKap.Service.Dtos.ShoppingCartDtos;
using KitKap.Service.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KitKap.Service.Services.Concretes
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public ShoppingCartService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        #region Add To Cart
        public async Task AddToCartAsync(string? userId, string? guestId, AddCartItemDto dto)
        {
            var cartRepo = _uow.GetRepository<ShoppingCart>();
            var productRepo = _uow.GetRepository<Product>();
            var cartItemRepo = _uow.GetRepository<ShoppingCartItem>();

            // ✅ Ürün kontrolü
            var product = await productRepo.GetByIdAsync(p => p.Id == dto.ProductId);
            if (product == null)
                throw new KeyNotFoundException("Ürün bulunamadı");

            // ✅ Kullanıcı veya misafir kimliği belirlenir
            var isUser = !string.IsNullOrEmpty(userId);
            var identifier = isUser ? userId : guestId;

            if (string.IsNullOrEmpty(identifier))
                throw new ArgumentException("Kullanıcı veya misafir kimliği belirtilmelidir.");

            // ✅ Sepeti getir (güncelleme için tracking açık)
            var cart = await cartRepo.GetWithIncludeForUpdateAsync(
                c => (isUser ? c.UserId == identifier : c.GuestId == identifier) && !c.IsCheckedOut,
                q => q.Include(c => c.Items)
            );

            // ✅ Sepet yoksa oluştur
            if (cart == null)
            {
                cart = new ShoppingCart
                {
                    UserId = isUser ? userId : null,
                    GuestId = !isUser ? guestId : null,
                    CreatedAt = DateTime.UtcNow,
                    IsCheckedOut = false,
                    Items = new List<ShoppingCartItem>()
                };
                await cartRepo.CreateAsync(cart);
                await _uow.CommitAsync();
            }

            cart.Items ??= new List<ShoppingCartItem>();

            // ✅ Sepette aynı ürün var mı kontrol et
            var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == dto.ProductId);

            if (existingItem != null)
            {
                // Mevcut ürünün miktarını artır
                existingItem.Quantity += dto.Quantity;
                await cartItemRepo.UpdateAsync(existingItem);
            }
            else
            {
                // Yeni ürün ekle
                var newItem = new ShoppingCartItem
                {
                    ShoppingCartId = cart.Id,
                    ProductId = dto.ProductId,
                    Quantity = dto.Quantity,
                    UnitPrice = product.Price
                };
                await cartItemRepo.CreateAsync(newItem);
            }

            await _uow.CommitAsync();
        }
        #endregion

        #region Get Cart
        public async Task<ShoppingCartDto> GetCartAsync(string? userId, string? guestId)
        {
            var cartRepo = _uow.GetRepository<ShoppingCart>();

            var isUser = !string.IsNullOrEmpty(userId);
            var identifier = isUser ? userId : guestId;

            if (string.IsNullOrEmpty(identifier))
                throw new ArgumentException("Kullanıcı veya misafir kimliği belirtilmelidir.");

            // ✅ Sepeti getir (sadece okuma, AsNoTracking ile)
            var cart = await cartRepo.GetWithIncludeAsync(
                c => (isUser ? c.UserId == identifier : c.GuestId == identifier) && !c.IsCheckedOut,
                q => q
                    .Include(c => c.Items)
                        .ThenInclude(i => i.Product)
                            .ThenInclude(p => p.ProductImages)
            );

            // ✅ Sepet yoksa yeni oluştur
            if (cart == null)
            {
                cart = new ShoppingCart
                {
                    UserId = isUser ? userId : null,
                    GuestId = !isUser ? guestId : null,
                    CreatedAt = DateTime.UtcNow,
                    IsCheckedOut = false,
                    Items = new List<ShoppingCartItem>()
                };

                await cartRepo.CreateAsync(cart);
                await _uow.CommitAsync();
            }

            cart.Items ??= new List<ShoppingCartItem>();

            // ✅ DTO dönüşümü
            var dto = new ShoppingCartDto
            {
                Id = cart.Id,
                UserId = cart.UserId,
                GuestId = cart.GuestId,
                CreatedAt = cart.CreatedAt,
                IsCheckedOut = cart.IsCheckedOut,
                Items = cart.Items.Select(i => new ShoppingCartItemDto
                {
                    Id = i.Id,
                    ProductId = i.ProductId,
                    ProductName = i.Product?.Name ?? "Bilinmeyen Ürün",
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    ProductImageUrl = GetProductImageUrl(i.Product)
                }).ToList()
            };

            return dto;
        }

        // ✅ Resim URL'sini güvenli şekilde al
        private string GetProductImageUrl(Product? product)
        {
            if (product?.ProductImages == null || !product.ProductImages.Any())
                return "/images/no-image.png";

            // Önce ana resmi bul
            var mainImage = product.ProductImages.FirstOrDefault(img => img.IsMain);
            if (mainImage != null)
                return mainImage.ImageUrl;

            // Ana resim yoksa ilk resmi döndür
            return product.ProductImages.First().ImageUrl;
        }
        #endregion

        #region Remove Item
        public async Task RemoveFromCartAsync(string? userId, string? guestId, long productId)
        {
            var cartRepo = _uow.GetRepository<ShoppingCart>();
            var cartItemRepo = _uow.GetRepository<ShoppingCartItem>();

            // ✅ userId veya guestId ile sepeti bul
            var isUser = !string.IsNullOrEmpty(userId);
            var identifier = isUser ? userId : guestId;

            if (string.IsNullOrEmpty(identifier))
                throw new ArgumentException("Kullanıcı veya misafir kimliği belirtilmelidir.");

            // ✅ Sepeti getir (güncelleme için tracking açık)
            var cart = await cartRepo.GetWithIncludeForUpdateAsync(
                c => (isUser ? c.UserId == identifier : c.GuestId == identifier) && !c.IsCheckedOut,
                q => q.Include(c => c.Items)
            );

            if (cart == null)
                throw new KeyNotFoundException("Sepet bulunamadı");

            // ✅ Ürünü sepetten bul
            var item = cart.Items?.FirstOrDefault(i => i.ProductId == productId);

            if (item == null)
                throw new KeyNotFoundException("Ürün sepette bulunamadı");

            // ✅ Ürünü sil
            await cartItemRepo.DeleteAsync(item);
            await _uow.CommitAsync();
        }
        #endregion

        #region Update Quantity
        public async Task UpdateQuantityAsync(string? userId, string? guestId, long productId, int newQuantity)
        {
            var cartRepo = _uow.GetRepository<ShoppingCart>();
            var cartItemRepo = _uow.GetRepository<ShoppingCartItem>();

            // ✅ userId veya guestId ile sepeti bul
            var isUser = !string.IsNullOrEmpty(userId);
            var identifier = isUser ? userId : guestId;

            if (string.IsNullOrEmpty(identifier))
                throw new ArgumentException("Kullanıcı veya misafir kimliği belirtilmelidir.");

            // ✅ Sepeti getir (güncelleme için tracking açık)
            var cart = await cartRepo.GetWithIncludeForUpdateAsync(
                c => (isUser ? c.UserId == identifier : c.GuestId == identifier) && !c.IsCheckedOut,
                q => q.Include(c => c.Items)
            );

            if (cart == null)
                throw new KeyNotFoundException("Sepet bulunamadı");

            // ✅ Ürünü sepetten bul
            var item = cart.Items?.FirstOrDefault(i => i.ProductId == productId);

            if (item == null)
                throw new KeyNotFoundException("Ürün sepette bulunamadı");

            // ✅ Miktar 0 veya negatifse sil, değilse güncelle
            if (newQuantity <= 0)
            {
                await cartItemRepo.DeleteAsync(item);
            }
            else
            {
                item.Quantity = newQuantity;
                await cartItemRepo.UpdateAsync(item);
            }

            await _uow.CommitAsync();
        }
        #endregion

        #region Total Price
        public async Task<decimal> GetTotalPriceAsync(string? userId, string? guestId)
        {
            var cart = await GetCartAsync(userId, guestId);
            return cart.Items.Sum(i => i.UnitPrice * i.Quantity);
        }
        #endregion

        #region Clear Cart
        public async Task ClearCartAsync(string? userId, string? guestId)
        {
            var cartRepo = _uow.GetRepository<ShoppingCart>();
            var cartItemRepo = _uow.GetRepository<ShoppingCartItem>();

            // ✅ userId veya guestId ile sepeti bul
            var isUser = !string.IsNullOrEmpty(userId);
            var identifier = isUser ? userId : guestId;

            if (string.IsNullOrEmpty(identifier))
                throw new ArgumentException("Kullanıcı veya misafir kimliği belirtilmelidir.");

            // ✅ Sepeti getir
            var cart = await cartRepo.GetWithIncludeForUpdateAsync(
                c => (isUser ? c.UserId == identifier : c.GuestId == identifier) && !c.IsCheckedOut,
                q => q.Include(c => c.Items)
            );

            if (cart == null || cart.Items == null || !cart.Items.Any())
                return;

            // ✅ Tüm ürünleri sil
            foreach (var item in cart.Items.ToList())
            {
                await cartItemRepo.DeleteAsync(item);
            }

            await _uow.CommitAsync();
        }
        #endregion

        #region Merge Guest Cart to User Cart (Bonus: Giriş yapınca sepetleri birleştir)
        public async Task MergeGuestCartToUserAsync(string userId, string guestId)
        {
            var cartRepo = _uow.GetRepository<ShoppingCart>();
            var cartItemRepo = _uow.GetRepository<ShoppingCartItem>();

            // ✅ Misafir sepetini çek
            var guestCart = await cartRepo.GetWithIncludeForUpdateAsync(
                c => c.GuestId == guestId && !c.IsCheckedOut,
                q => q.Include(c => c.Items)
            );

            if (guestCart == null || !guestCart.Items.Any())
                return; // Misafir sepeti boş

            // ✅ Kullanıcı sepetini çek
            var userCart = await cartRepo.GetWithIncludeForUpdateAsync(
                c => c.UserId == userId && !c.IsCheckedOut,
                q => q.Include(c => c.Items)
            );

            if (userCart == null)
            {
                // Kullanıcı sepeti yoksa, misafir sepetini kullanıcıya ata
                guestCart.UserId = userId;
                guestCart.GuestId = null;
                await cartRepo.UpdateAsync(guestCart);
            }
            else
            {
                // İki sepeti birleştir
                foreach (var guestItem in guestCart.Items.ToList())
                {
                    var existingItem = userCart.Items.FirstOrDefault(i => i.ProductId == guestItem.ProductId);

                    if (existingItem != null)
                    {
                        // Aynı ürün varsa miktarı topla
                        existingItem.Quantity += guestItem.Quantity;
                        await cartItemRepo.UpdateAsync(existingItem);
                    }
                    else
                    {
                        // Yeni ürünse kullanıcı sepetine ekle
                        guestItem.ShoppingCartId = userCart.Id;
                        await cartItemRepo.UpdateAsync(guestItem);
                    }
                }

                // Misafir sepetini sil
                await cartRepo.DeleteAsync(guestCart);
            }

            await _uow.CommitAsync();
        }
        #endregion
    }
}
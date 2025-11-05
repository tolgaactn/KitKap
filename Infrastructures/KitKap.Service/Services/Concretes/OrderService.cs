using AutoMapper;
using Kitkap.Entity.Entities;
using Kitkap.Entity.UnitOfWorks;
using Kitkap.Service.Dtos.AddressDtos;
using KitKap.DataAccess.Identity;
using KitKap.Service.Dtos.OrderDtos;
using KitKap.Service.Extensions;
using KitKap.Service.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static Kitkap.Entity.Entities.Order;

namespace KitKap.Service.Services.Concretes
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        private const decimal SHIPPING_COST = 50m;
        private const decimal FREE_SHIPPING_THRESHOLD = 400m;
        private const decimal COMMISSION_RATE = 0.10m;
        public OrderService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _uow = unitOfWork;
            _mapper = mapper;
        }

        public async Task<int> CreateOrderFromCartAsync(CreateOrderDto dto)
        {
            var cart = await _uow.GetRepository<ShoppingCart>().GetWithIncludeAsync(
                filter: c => c.UserId == dto.BuyerId && !c.IsCheckedOut,
                include: q => q
                    .Include(c => c.Items)
                        .ThenInclude(i => i.Product)
            );

            if (cart == null || cart.Items == null || !cart.Items.Any())
                throw new InvalidOperationException("Sepetiniz boş");

            // 2. STOK KONTROLÜ
            var unavailableProducts = new List<string>();

            foreach (var item in cart.Items)
            {
                var product = item.Product;

                // Stok kontrolü
                if (product.Stock < item.Quantity)
                {
                    unavailableProducts.Add($"{product.Name} (Stok: {product.Stock}, İstenen: {item.Quantity})");
                }

                // Ürün satışta mı kontrolü
                if (product.Status == Product.ProductStatus.OutOfStock ||
                    product.Status == Product.ProductStatus.Discontinued)
                {
                    unavailableProducts.Add($"{product.Name} (Artık satışta değil)");
                }
            }

            if (unavailableProducts.Any())
            {
                throw new InvalidOperationException(
                    $"Şu ürünler sepetinizden kaldırıldı çünkü stokta yok:\n{string.Join("\n", unavailableProducts)}"
                );
            }

            // 3. ADRES KONTROLÜ
            var address = await _uow.GetRepository<Address>().GetByIdAsync(dto.ShippingAddressId);
            if (address == null)
                throw new KeyNotFoundException("Teslimat adresi bulunamadı");

            // 4. FİYAT HESAPLAMALARI
            decimal subTotal = 0;
            decimal totalCommission = 0;

            var orderItems = new List<OrderItem>();

            foreach (var cartItem in cart.Items)
            {
                var product = cartItem.Product;
                var itemTotal = cartItem.UnitPrice * cartItem.Quantity;
                var itemCommission = itemTotal * COMMISSION_RATE;
                var sellerAmount = itemTotal - itemCommission;

                var orderItem = new OrderItem
                {
                    ProductId = cartItem.ProductId,
                    SellerId = product.OwnerId,
                    Quantity = cartItem.Quantity,
                    UnitPrice = cartItem.UnitPrice,
                    CommissionRate = COMMISSION_RATE,
                    CommissionAmount = itemCommission,
                    SellerAmount = sellerAmount,
                    Status = OrderItem.OrderItemStatus.Pending
                };

                orderItems.Add(orderItem);

                subTotal += itemTotal;
                totalCommission += itemCommission;
            }

            // 5. KARGO ÜCRETİ HESAPLA
            var shippingCost = subTotal >= FREE_SHIPPING_THRESHOLD ? 0 : SHIPPING_COST;
            var totalAmount = subTotal + shippingCost;

            // 6. ORDER OLUŞTUR
            var order = new Order
            {
                BuyerId = dto.BuyerId,
                ShippingAddressId = dto.ShippingAddressId,
                PaymentMethod = dto.PaymentMethod,
                Status = OrderStatus.Pending,
                Items = orderItems,
                SubTotal = subTotal,
                ShippingCost = shippingCost,
                CommissionAmount = totalCommission,
                TotalAmount = totalAmount,
                CustomerNote = dto.CustomerNote,
                CreatedAt = DateTime.UtcNow
            };

            await _uow.GetRepository<Order>().CreateAsync(order);
            await _uow.CommitAsync(); // ✅ Order.Id oluşması için commit

            // 7. STOK DÜŞÜR
            foreach (var item in cart.Items)
            {
                var product = item.Product;
                product.Stock -= item.Quantity;

                // Stok 0 olduysa durumu güncelle
                if (product.Stock == 0)
                {
                    product.Status = Product.ProductStatus.OutOfStock;
                }

                await _uow.GetRepository<Product>().UpdateAsync(product);
            }

            // 8. SEPETİ CHECKOUT YAP
            cart.IsCheckedOut = true;
            await _uow.GetRepository<ShoppingCart>().UpdateAsync(cart);

            // 9. TRANSACTION OLUŞTUR
            var transactionService = new TransactionService(_uow, _mapper);
            var createTransactionDto = new CreateTransactionDto
            {
                OrderId = order.Id,
                PaymentMethod = dto.PaymentMethod,
                PaymentProvider = dto.PaymentMethod == "CreditCard" ? "PayTR" : null
            };

            await transactionService.CreateTransactionForOrderAsync(createTransactionDto);

            // 10. FINAL COMMIT
            await _uow.CommitAsync();

            return order.Id;
        
        }

        public async Task<IEnumerable<OrderDto>> GetOrdersByUserAsync(string userId)
        {
            // 1. Kullanıcının tüm siparişlerini çek
            var orders = await _uow.GetRepository<Order>().GetAll(
                filter: o => o.BuyerId == userId,
                orderby: q => q.OrderByDescending(o => o.CreatedAt),
                includes: new Expression<Func<Order, object>>[]
                {
            o => o.Items
                }
            );

            if (!orders.Any())
                return new List<OrderDto>();

            // 2. Buyer ve Address'leri çek (tek seferde)
            var buyer = await _uow.GetRepository<AppUser>().GetByIdAsync(userId);

            var addressIds = orders.Select(o => o.ShippingAddressId).Distinct().ToList();
            var addresses = await _uow.GetRepository<Address>().GetAll(
                filter: a => addressIds.Contains(a.AddressId)
            );

            // 3. DTO'lara çevir
            var dtoList = new List<OrderDto>();

            foreach (var order in orders)
            {
                var dto = _mapper.Map<OrderDto>(order);
                var address = addresses.FirstOrDefault(a => a.AddressId == order.ShippingAddressId);

                dto.EnrichDto(order, buyer, address);

                dtoList.Add(dto);
            }

            return dtoList;
        }

        public async Task<OrderSummaryDto> GetOrderSummaryAsync(string userId)
        {
            var cart = await _uow.GetRepository<ShoppingCart>().GetWithIncludeAsync(
                filter: c => c.UserId == userId && !c.IsCheckedOut,
                include: q => q.Include(c => c.Items).ThenInclude(i => i.Product).ThenInclude(p => p.ProductImages));

            if (cart == null || cart.Items == null || !cart.Items.Any())
                throw new InvalidOperationException("Sepetiniz boş");

            var orderItems = new List<OrderItemDto>();

            foreach (var cartItem in cart.Items)
            {
                // Product ve Owner bilgilerini çek
                var product = cartItem.Product;
                var seller = await _uow.GetRepository<AppUser>().GetByIdAsync(product.OwnerId);

                var itemDto = new OrderItemDto
                {
                    ProductId = cartItem.ProductId,
                    ProductName = product.Name,
                    ProductImageUrl = product.GetMainImageUrl(),
                    SellerId = product.OwnerId,
                    SellerName = seller != null ? $"{seller.FirstName} {seller.LastName}" : "Bilinmeyen",
                    Quantity = cartItem.Quantity,
                    UnitPrice = cartItem.UnitPrice,
                    CommissionRate = COMMISSION_RATE,
                    CommissionAmount = cartItem.UnitPrice * cartItem.Quantity * COMMISSION_RATE,
                    SellerAmount = cartItem.UnitPrice * cartItem.Quantity * (1 - COMMISSION_RATE),
                    Status = OrderItem.OrderItemStatus.Pending,
                    StatusText = "Bekliyor"
                };

                orderItems.Add(itemDto);
            }

            var subTotal = orderItems.Sum(i => i.UnitPrice * i.Quantity);
            var shippingCost = subTotal >= FREE_SHIPPING_THRESHOLD ? 0 : SHIPPING_COST;
            var totalAmount = subTotal + shippingCost;

            // 4. DTO oluştur
            var summary = new OrderSummaryDto
            {
                Items = orderItems,
                SubTotal = subTotal,
                ShippingCost = shippingCost,
                TotalAmount = totalAmount,
                IsFreeShipping = shippingCost == 0,
                FreeShippingThreshold = FREE_SHIPPING_THRESHOLD,
                RemainingForFreeShipping = shippingCost > 0 ? FREE_SHIPPING_THRESHOLD - subTotal : 0
            };

            return summary;
        }

        // ========================================
        // 5. SİPARİŞ DURUMU GÜNCELLE
        // ========================================

        public async Task UpdateOrderStatusAsync(int orderId, OrderStatus newStatus)
        {
            var order = await _uow.GetRepository<Order>().GetByIdAsync(orderId);

            if (order == null)
                throw new KeyNotFoundException("Sipariş bulunamadı");

            order.Status = newStatus;

            // Durum güncellemelerine göre tarihleri ayarla
            switch (newStatus)
            {
                case OrderStatus.Shipped:
                    order.ShippedAt = DateTime.UtcNow;
                    break;
                case OrderStatus.Delivered:
                    order.DeliveredAt = DateTime.UtcNow;
                    break;
            }

            await _uow.GetRepository<Order>().UpdateAsync(order);
            await _uow.CommitAsync();
        }

        // ========================================
        // 6. KARGO BİLGİLERİNİ GÜNCELLE
        // ========================================

        public async Task UpdateShippingInfoAsync(int orderId, string trackingNumber, string cargoCompany)
        {
            var order = await _uow.GetRepository<Order>().GetByIdAsync(orderId);

            if (order == null)
                throw new KeyNotFoundException("Sipariş bulunamadı");

            order.TrackingNumber = trackingNumber;
            order.CargoCompany = cargoCompany;
            order.Status = OrderStatus.Shipped;
            order.ShippedAt = DateTime.UtcNow;

            await _uow.GetRepository<Order>().UpdateAsync(order);
            await _uow.CommitAsync();
        }

        public async Task<OrderDto> GetOrderByIdAsync(int orderId)
        {
            // 1. Order'ı çek (Items dahil)
            var order = await _uow.GetRepository<Order>().GetWithIncludeAsync(
                filter: o => o.Id == orderId,
                include: q => q.Include(o => o.Items)
                        .ThenInclude(i => i.Product)
                            .ThenInclude(p => p.ProductImages)
                    .Include(o => o.Transaction)
            );

            if (order == null)
                throw new KeyNotFoundException("Sipariş bulunamadı");

            // 2. Buyer ve Address'i ayrı çek (navigation property olmadığı için)
            var buyer = await _uow.GetRepository<AppUser>().GetByIdAsync(order.BuyerId);
            var address = await _uow.GetRepository<Address>().GetByIdAsync(order.ShippingAddressId);

            // 3. Seller'ları çek (OrderItem'lardaki SellerId'ler için)
            var sellerIds = order.Items.Select(i => i.SellerId).Distinct().ToList();
            var sellers = await _uow.GetRepository<AppUser>().GetAll(
                filter: u => sellerIds.Contains(u.Id)
            );

            // 4. Order → OrderDto mapping
            var dto = _mapper.Map<OrderDto>(order);

            // 5. Extension ile zenginleştir (Buyer ve Address bilgilerini ekle)
            dto.EnrichDto(order, buyer, address);

            // 6. Her OrderItem'ı zenginleştir
            foreach (var itemDto in dto.Items)
            {
                var item = order.Items.First(i => i.Id == itemDto.Id);
                var product = item.Product;
                var seller = sellers.FirstOrDefault(s => s.Id == item.SellerId);

                itemDto.EnrichDto(item, product, seller);
            }

            return dto;
        }
    }
}

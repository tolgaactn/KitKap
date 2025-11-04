using AutoMapper;
using Kitkap.Entity.Entities;
using Kitkap.Entity.Services;
using Kitkap.Entity.UnitOfWorks;
using Kitkap.Service.Dtos.AddressDtos;
using KitKap.Service.Dtos.TransactionDtos;
using KitKap.Service.Extensions;
using KitKap.Service.Services.Interfaces;
using static Kitkap.Entity.Entities.Transaction;

namespace KitKap.Service.Services.Concretes
{
    public class TransactionService : ITransactionService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public TransactionService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        // ========================================
        // YENİ SİSTEM: ORDER-BASED TRANSACTION
        // ========================================

        /// <summary>
        /// Sipariş için transaction oluşturur
        /// </summary>
        public async Task<int> CreateTransactionForOrderAsync(CreateTransactionDto dto)
        {
            var orderRepo = _uow.GetRepository<Order>();
            var transactionRepo = _uow.GetRepository<Transaction>();

            // 1. Siparişi kontrol et
            var order = await orderRepo.GetByIdAsync(dto.OrderId);
            if (order == null)
                throw new KeyNotFoundException("Sipariş bulunamadı");

            // 2. Bu sipariş için zaten transaction var mı kontrol et
            var existingTransaction = await transactionRepo.Get(
                filter: t => t.OrderId == dto.OrderId
            );

            if (existingTransaction != null)
                throw new InvalidOperationException("Bu sipariş için zaten bir ödeme kaydı mevcut");

            // 3. Yeni transaction oluştur
            var transaction = new Transaction
            {
                OrderId = dto.OrderId,
                PaymentMethod = dto.PaymentMethod,
                Amount = order.TotalAmount,
                Status = PaymentStatus.Pending,
                PaymentProvider = dto.PaymentProvider,
                CreatedAt = DateTime.UtcNow
            };

            await transactionRepo.CreateAsync(transaction);
            await _uow.CommitAsync();

            return transaction.Id;
        }

        /// <summary>
        /// Ödeme tamamlandığında çağrılır (Havale onayı veya kredi kartı başarılı)
        /// </summary>
        public async Task CompleteTransactionAsync(int transactionId, string? gatewayTransactionId = null)
        {
            var transactionRepo = _uow.GetRepository<Transaction>();
            var orderRepo = _uow.GetRepository<Order>();

            // 1. Transaction'ı bul
            var transaction = await transactionRepo.GetByIdAsync(transactionId);
            if (transaction == null)
                throw new KeyNotFoundException("Ödeme kaydı bulunamadı");

            if (transaction.Status == PaymentStatus.Completed)
                throw new InvalidOperationException("Bu ödeme zaten tamamlanmış");

            // 2. Transaction'ı tamamla
            transaction.Status = PaymentStatus.Completed;
            transaction.CompletedAt = DateTime.UtcNow;
            transaction.TransactionId = gatewayTransactionId;

            await transactionRepo.UpdateAsync(transaction);

            // 3. Order durumunu güncelle
            var order = await orderRepo.GetByIdAsync(transaction.OrderId);
            if (order != null)
            {
                order.Status = Order.OrderStatus.PaymentReceived;
                await orderRepo.UpdateAsync(order);
            }

            await _uow.CommitAsync();
        }

        /// <summary>
        /// Ödeme başarısız olduğunda çağrılır
        /// </summary>
        public async Task FailTransactionAsync(int transactionId, string errorMessage)
        {
            var transactionRepo = _uow.GetRepository<Transaction>();

            var transaction = await transactionRepo.GetByIdAsync(transactionId);
            if (transaction == null)
                throw new KeyNotFoundException("Ödeme kaydı bulunamadı");

            transaction.Status = PaymentStatus.Failed;
            transaction.ErrorMessage = errorMessage;

            await transactionRepo.UpdateAsync(transaction);
            await _uow.CommitAsync();
        }

        /// <summary>
        /// Ödeme iptal edildiğinde çağrılır
        /// </summary>
        public async Task CancelTransactionAsync(int transactionId)
        {
            var transactionRepo = _uow.GetRepository<Transaction>();
            var orderRepo = _uow.GetRepository<Order>();

            var transaction = await transactionRepo.GetByIdAsync(transactionId);
            if (transaction == null)
                throw new KeyNotFoundException("Ödeme kaydı bulunamadı");

            transaction.Status = PaymentStatus.Cancelled;

            await transactionRepo.UpdateAsync(transaction);

            // Order'ı da iptal et
            var order = await orderRepo.GetByIdAsync(transaction.OrderId);
            if (order != null)
            {
                order.Status = Order.OrderStatus.Cancelled;
                await orderRepo.UpdateAsync(order);
            }

            await _uow.CommitAsync();
        }

        /// <summary>
        /// İade işlemi
        /// </summary>
        public async Task RefundTransactionAsync(int transactionId)
        {
            var transactionRepo = _uow.GetRepository<Transaction>();
            var orderRepo = _uow.GetRepository<Order>();

            var transaction = await transactionRepo.GetByIdAsync(transactionId);
            if (transaction == null)
                throw new KeyNotFoundException("Ödeme kaydı bulunamadı");

            if (transaction.Status != PaymentStatus.Completed)
                throw new InvalidOperationException("Sadece tamamlanmış ödemeler iade edilebilir");

            transaction.Status = PaymentStatus.Refunded;
            await transactionRepo.UpdateAsync(transaction);

            // Order durumunu güncelle
            var order = await orderRepo.GetByIdAsync(transaction.OrderId);
            if (order != null)
            {
                order.Status = Order.OrderStatus.Refunded;
                await orderRepo.UpdateAsync(order);
            }

            await _uow.CommitAsync();
        }

        /// <summary>
        /// Transaction ID'ye göre getir
        /// </summary>
        public async Task<TransactionDto?> GetTransactionByIdAsync(int transactionId)
        {
            var transaction = await _uow.GetRepository<Transaction>().GetByIdAsync(transactionId);

            if (transaction == null)
                return null;

            var dto = _mapper.Map<TransactionDto>(transaction);

            // Extension ile zenginleştir
            dto.EnrichDto(transaction);

            return dto;
        }

        /// <summary>
        /// Order ID'ye göre transaction getir
        /// </summary>
        public async Task<TransactionDto?> GetTransactionByOrderIdAsync(int orderId)
        {
            var transaction = await _uow.GetRepository<Transaction>().Get(
                filter: t => t.OrderId == orderId
            );

            if (transaction == null)
                return null;

            var dto = _mapper.Map<TransactionDto>(transaction);

            // Extension ile zenginleştir
            dto.EnrichDto(transaction);

            return dto;
        }

        /// <summary>
        /// Tüm transaction'ları getir (Admin için)
        /// </summary>
        public async Task<IEnumerable<TransactionDto>> GetAllTransactionsAsync()
        {
            var transactions = await _uow.GetRepository<Transaction>().GetAllAsync();

            var dtoList = _mapper.Map<List<TransactionDto>>(transactions);

            // Her birini zenginleştir
            var transactionsList = transactions.ToList();
            for (int i = 0; i < dtoList.Count; i++)
            {
                dtoList[i].EnrichDto(transactionsList[i]);
            }

            return dtoList;
        }

        /// <summary>
        /// Bekleyen ödemeleri getir (Havale onayı için)
        /// </summary>
        public async Task<IEnumerable<TransactionDto>> GetPendingTransactionsAsync()
        {
            var transactions = await _uow.GetRepository<Transaction>().GetAll(
                filter: t => t.Status == PaymentStatus.Pending && t.PaymentMethod == "BankTransfer"
            );

            var dtoList = _mapper.Map<List<TransactionDto>>(transactions.ToList());

            // Her birini zenginleştir
            var transactionsList = transactions.ToList();
            for (int i = 0; i < dtoList.Count; i++)
            {
                dtoList[i].EnrichDto(transactionsList[i]);
            }

            return dtoList;
        }
    }
}
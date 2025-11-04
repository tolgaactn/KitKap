using Kitkap.Service.Dtos.AddressDtos;
using KitKap.Service.Dtos.TransactionDtos;

namespace KitKap.Service.Services.Interfaces
{
    public interface ITransactionService
    {
        // Transaction oluşturma
        Task<int> CreateTransactionForOrderAsync(CreateTransactionDto dto);

        // Transaction durum güncellemeleri
        Task CompleteTransactionAsync(int transactionId, string? gatewayTransactionId = null);
        Task FailTransactionAsync(int transactionId, string errorMessage);
        Task CancelTransactionAsync(int transactionId);
        Task RefundTransactionAsync(int transactionId);

        // Transaction okuma
        Task<TransactionDto?> GetTransactionByIdAsync(int transactionId);
        Task<TransactionDto?> GetTransactionByOrderIdAsync(int orderId);
        Task<IEnumerable<TransactionDto>> GetAllTransactionsAsync();
        Task<IEnumerable<TransactionDto>> GetPendingTransactionsAsync();
    }
}
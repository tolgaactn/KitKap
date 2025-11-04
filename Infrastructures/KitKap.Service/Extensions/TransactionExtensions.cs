using Kitkap.Entity.Entities;
using KitKap.Service.Dtos.TransactionDtos;
using static Kitkap.Entity.Entities.Transaction;

namespace KitKap.Service.Extensions
{
    /// <summary>
    /// Transaction entity için extension metodlar
    /// </summary>
    public static class TransactionExtensions
    {
        /// <summary>
        /// PaymentStatus'ü Türkçe metne çevirir
        /// </summary>
        public static string ToDisplayText(this PaymentStatus status)
        {
            return status switch
            {
                PaymentStatus.Pending => "Bekliyor",
                PaymentStatus.Processing => "İşleniyor",
                PaymentStatus.Completed => "Tamamlandı",
                PaymentStatus.Failed => "Başarısız",
                PaymentStatus.Refunded => "İade Edildi",
                PaymentStatus.Cancelled => "İptal Edildi",
                _ => "Bilinmeyen"
            };
        }

        /// <summary>
        /// PaymentStatus için CSS class döndürür
        /// </summary>
        public static string ToStatusClass(this PaymentStatus status)
        {
            return status switch
            {
                PaymentStatus.Pending => "badge-warning",
                PaymentStatus.Processing => "badge-info",
                PaymentStatus.Completed => "badge-success",
                PaymentStatus.Failed => "badge-danger",
                PaymentStatus.Refunded => "badge-secondary",
                PaymentStatus.Cancelled => "badge-dark",
                _ => "badge-light"
            };
        }

        /// <summary>
        /// TransactionDto'yu zenginleştirir
        /// </summary>
        public static void EnrichDto(this TransactionDto dto, Transaction transaction)
        {
            dto.StatusText = transaction.Status.ToDisplayText();
            dto.PaymentMethodText = transaction.PaymentMethod.ToDisplayText();
        }
    }
}
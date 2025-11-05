using KitKap.Service.Dtos.OrderDtos;
using KitKap.Service.Dtos.TransactionDtos;

namespace KitKap.MvcUI.ViewModels.CheckoutViewModels
{
    public class OrderConfirmationViewModel
    {
        public OrderDto Order { get; set; }
        public TransactionDto? Transaction { get; set; }

        // Ödeme talimatları (Havale/EFT için)
        public string? PaymentInstructions { get; set; }

        // Başarı mesajı
        public string SuccessMessage { get; set; } = "Siparişiniz başarıyla oluşturuldu!";
    }
}
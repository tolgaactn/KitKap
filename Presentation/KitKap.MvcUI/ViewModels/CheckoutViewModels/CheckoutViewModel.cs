using Kitkap.Service.Dtos.AddressDtos;
using KitKap.Service.Dtos.OrderDtos;

namespace KitKap.MvcUI.ViewModels.CheckoutViewModels
{
    public class CheckoutViewModel
    {
        // Sipariş özeti
        public OrderSummaryDto OrderSummary { get; set; }

        // Kullanıcının adresleri
        public List<RequestAddressDto> UserAddresses { get; set; } = new();

        // Seçili adres ID
        public int SelectedAddressId { get; set; }

        // Yeni adres ekleme (opsiyonel)
        public CreateAddressDto? NewAddress { get; set; }

        // Ödeme yöntemi
        public string PaymentMethod { get; set; } = "BankTransfer";

        // Müşteri notu
        public string? CustomerNote { get; set; }

        // Kullanıcı bilgileri
        public string UserId { get; set; }
        public string UserEmail { get; set; }
        public string UserFullName { get; set; }
    }
}
using Kitkap.Entity.Entities;

namespace KitKap.Service.Extensions
{
    /// <summary>
    /// Address entity için extension metodlar
    /// </summary>
    public static class AddressExtensions
    {
        /// <summary>
        /// Address'i okunabilir formatta string'e çevirir
        /// </summary>
        public static string ToDisplayText(this Address? address)
        {
            if (address == null)
                return "Adres bilgisi yok";

            var parts = new List<string>();

            if (!string.IsNullOrEmpty(address.Description))
                parts.Add(address.Description);

            if (!string.IsNullOrEmpty(address.District))
                parts.Add(address.District);

            if (!string.IsNullOrEmpty(address.City))
                parts.Add(address.City);

            if (!string.IsNullOrEmpty(address.Country))
                parts.Add(address.Country);

            if (address.PostCode > 0)
                parts.Add($"PK: {address.PostCode}");

            return string.Join(", ", parts);
        }

        /// <summary>
        /// Address'i kısa formatta gösterir (sadece ilçe/şehir)
        /// </summary>
        public static string ToShortText(this Address? address)
        {
            if (address == null)
                return "Adres yok";

            return $"{address.District}, {address.City}";
        }
    }
}
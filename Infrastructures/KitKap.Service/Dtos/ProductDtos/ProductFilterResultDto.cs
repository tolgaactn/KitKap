using Kitkap.Service.Dtos.AddressDtos;
using System.Collections.Generic;

namespace KitKap.Service.Dtos.ProductDtos
{
    public class ProductFilterResultDto
    {
        // Filtrelenmiş ürünler
        public List<RequestProductDto> Products { get; set; } = new();

        // Sayfalama bilgileri
        public int TotalProducts { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }

        // Fiyat aralığı (tüm ürünler için)
        public decimal MinAvailablePrice { get; set; }
        public decimal MaxAvailablePrice { get; set; }
    }
}
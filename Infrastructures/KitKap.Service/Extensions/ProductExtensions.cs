using Kitkap.Entity.Entities;

namespace KitKap.Service.Extensions
{
    /// <summary>
    /// Product entity için extension metodlar
    /// </summary>
    public static class ProductExtensions
    {
        /// <summary>
        /// Ürünün ana resmini döndürür
        /// </summary>
        public static string GetMainImageUrl(this Product? product)
        {
            if (product?.ProductImages == null || !product.ProductImages.Any())
                return "/images/no-image.png";

            // Önce IsMain=true olanı ara
            var mainImage = product.ProductImages.FirstOrDefault(img => img.IsMain);
            if (mainImage != null)
                return mainImage.ImageUrl;

            // Yoksa ilk resmi döndür
            return product.ProductImages.First().ImageUrl;
        }

        /// <summary>
        /// ProductStatus'ü Türkçe metne çevirir
        /// </summary>
        public static string ToDisplayText(this Product.ProductStatus status)
        {
            return status switch
            {
                Product.ProductStatus.OutOfStock => "Stokta Yok",
                Product.ProductStatus.InStock => "Stokta Var",
                Product.ProductStatus.Discontinued => "Satıştan Kaldırıldı",
                _ => "Bilinmeyen"
            };
        }

        /// <summary>
        /// ProductCondition'ı Türkçe metne çevirir
        /// </summary>
        public static string ToDisplayText(this Product.ProductCondition condition)
        {
            return condition switch
            {
                Product.ProductCondition.New => "Sıfır/Yeni",
                Product.ProductCondition.LikeNew => "Sıfır Ayarında",
                Product.ProductCondition.VeryGood => "Çok İyi",
                Product.ProductCondition.Good => "İyi",
                Product.ProductCondition.Acceptable => "Kabul Edilebilir",
                _ => "Bilinmeyen"
            };
        }

        /// <summary>
        /// ProductApprovalStatus'ü Türkçe metne çevirir
        /// </summary>
        public static string ToDisplayText(this Product.ProductApprovalStatus status)
        {
            return status switch
            {
                Product.ProductApprovalStatus.Pending => "Onay Bekliyor",
                Product.ProductApprovalStatus.Approved => "Onaylandı",
                Product.ProductApprovalStatus.Rejected => "Reddedildi",
                _ => "Bilinmeyen"
            };
        }
    }
}
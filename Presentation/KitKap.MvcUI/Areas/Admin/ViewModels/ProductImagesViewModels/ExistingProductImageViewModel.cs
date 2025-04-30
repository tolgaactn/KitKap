namespace KitKap.MvcUI.Areas.Admin.ViewModels.ProductImagesViewModels
{
    public class ExistingProductImageViewModel
    {
        public long Id { get; set; }
        public string ImageUrl { get; set; }
        public string AltText { get; set; }
        public bool IsMain { get; set; }
    }
}

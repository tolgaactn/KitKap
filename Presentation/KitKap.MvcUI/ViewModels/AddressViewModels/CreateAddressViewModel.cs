namespace KitKap.MvcUI.ViewModels.AddressViewModels
{
    public class CreateAddressViewModel
    {
        public string Country { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public int PostCode { get; set; }
        public string? Description { get; set; }
    }
}

using KitKap.MvcUI.ViewModels.ShoppingCartViewModels;

namespace KitKap.MvcUI.ViewModels.NavbarViewModels
{
    public class NavbarViewModel
    {
        public ShoppingCartViewModel Cart { get; set; } = new();
        public bool IsSignedIn { get; set; }
        public string? UserFirstName { get; set; }
        public string? UserFullName { get; set; }
    }
}

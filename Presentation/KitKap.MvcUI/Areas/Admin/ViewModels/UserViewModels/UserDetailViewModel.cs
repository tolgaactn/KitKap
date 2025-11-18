namespace KitKap.MvcUI.Areas.Admin.ViewModels.UserViewModels
{
    public class UserDetailViewModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public decimal Balance { get; set; }
        public List<string> Roles { get; set; } = new();
        public DateTime? CreatedDate { get; set; }
        public bool EmailConfirmed { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public DateTime? LockoutEnd { get; set; }
        public bool LockoutEnabled { get; set; }
        public int AccessFailedCount { get; set; }

        // Kullanıcının siparişleri
        public int TotalOrders { get; set; }
        public decimal TotalSpent { get; set; }

        // Son siparişler
        public List<UserOrderSummaryViewModel> RecentOrders { get; set; } = new();

    }
}

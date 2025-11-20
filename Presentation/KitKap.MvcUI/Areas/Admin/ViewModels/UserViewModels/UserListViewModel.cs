namespace KitKap.MvcUI.Areas.Admin.ViewModels.UserViewModels
{
    public class UserListViewModel
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
        public bool IsLocked { get; set; }
    }
}

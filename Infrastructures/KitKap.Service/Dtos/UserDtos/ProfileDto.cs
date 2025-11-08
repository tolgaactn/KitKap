namespace Kitkap.Service.Dtos.UserDtos
{
    public class ProfileDto
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string UserName { get; set; }
        public decimal Balance { get; set; }
        public bool IsActived { get; set; }
    }
}
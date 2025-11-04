
using Kitkap.Service.Dtos.AddressDtos;
using Kitkap.Service.Dtos.UserDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kitkap.Entity.Services
{
    public interface IAccountService
    {
        Task<string> CreateUserAsync(RegisterUserDto model);
        Task<AuthResponse> LoginAsync(LoginUserDto model);
        Task<GetByIdUserDto> FindById(string id);
        Task<List<RequestUserDto>> GetAllUsersAsync();
        Task<RequestUserDto?> GetUserByEmailAsync(string email);
        Task<IList<string>> GetRolesAsync(string email);
        Task UpdateUserAsync(UpdateUserDto model);
        Task DeactivateUserAsync(DeactivateUserDto model);
        Task LogoutAsync();
    }
}

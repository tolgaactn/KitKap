
using Kitkap.Service.Dtos.AddressDtos;
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
        Task UpdateUserAsync(UpdateUserDto model);
        Task DeactivateUserAsync(DeactivateUserDto model);

    }
}

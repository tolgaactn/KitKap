using Kitkap.Entity.ViewModels.UserViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kitkap.Entity.Services
{
    public interface IAccountService
    {
        Task<string> CreateUserAsync(RegisterUserViewModel model);
        Task<AuthResponse> LoginAsync(LoginUserViewModel model);
        Task<GetByIdUserViewModel> FindById(string id);
        Task<List<RequestUserViewModel>> GetAllUsersAsync();
        Task UpdateUserAsync(UpdateUserViewModel model);
        Task DeactivateUserAsync(DeactivateUserViewModel model);

    }
}

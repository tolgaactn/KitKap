using Kitkap.Entity.ViewModels.AddressViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kitkap.Entity.Services
{
    public interface IAddressService
    {
        Task<IEnumerable<RequestAddressViewModel>> GetAllAddresses();
        Task<GetByIdAddressViewModel> GetByIdAddress(int id);
        Task AddAsync(CreateAddressViewModel model);
        Task DeleteAsync(RemoveAddressViewModel model);
        Task UpdateAsync(UpdateAddressViewModel model);
    }
}

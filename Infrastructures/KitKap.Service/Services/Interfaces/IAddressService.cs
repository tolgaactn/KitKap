using Kitkap.Service.Dtos.AddressDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kitkap.Entity.Services
{
    public interface IAddressService
    {
        Task<IEnumerable<RequestAddressDto>> GetAllAddresses();
        Task<GetByIdAddressDto> GetByIdAddress(int id);
        Task AddAsync(CreateAddressDto model);
        Task DeleteAsync(RemoveAddressDto model);
        Task UpdateAsync(UpdateAddressDto model);
    }
}

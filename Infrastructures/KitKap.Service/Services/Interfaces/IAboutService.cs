using Kitkap.Service.Dtos.AddressDtos;
using KitKap.Service.Dtos.AboutDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KitKap.Service.Services.Interfaces
{
    public interface IAboutService
    {
        Task<IEnumerable<ResultAboutDto>> GetAllAboutAsync();
        Task CreateAboutAsync(CreateAboutDto createAboutDto);
        Task UpdateAboutAsync(UpdateAboutDto updateAboutDto);
        Task DeleteAboutAsync(int id);
        Task<GetByIdAboutDto> GetByIdAboutAsync(int id);
        
        
        
    }
}

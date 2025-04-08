using Kitkap.Service.Dtos.AddressDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kitkap.Entity.Services
{
    public interface ICategoryService
    {
        Task<IEnumerable<ResultCategoryDto>> GetAllCategories();
        Task<GetByIdCategoryDto> GetByIdCategory(int id);
        Task AddAsync(CreateCategoryDto model);
        Task DeleteAsync(int id);
        Task UpdateAsync(UpdateCategoryDto model);
    }
}

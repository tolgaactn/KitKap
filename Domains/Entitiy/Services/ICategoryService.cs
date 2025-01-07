using Kitkap.Entity.ViewModels.CategoryViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kitkap.Entity.Services
{
    public interface ICategoryService
    {
        Task<IEnumerable<RequestCategoryViewModel>> GetAllCategories();
        Task<GetByIdCategoryViewModel> GetByIdCategory(int id);
        Task AddAsync(CreateCategoryViewModel model);
        Task DeleteAsync(RemoveCategoryViewModel model);
        Task UpdateAsync(UpdateCategoryViewModel model);
    }
}

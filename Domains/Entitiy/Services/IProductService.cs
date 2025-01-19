using Kitkap.Entity.ViewModels.BookViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kitkap.Entity.Services
{
    public interface IProductService
    {
       Task<IEnumerable<RequestProductViewModel>> GetAllProducts();
       Task<GetByIdProductViewModel> GetByIdProduct(long id);
       Task AddAsync(CreateProductViewModel model);
       Task DeleteAsync(RemoveProductViewModel model);
       Task UpdateAsync(UpdateProductViewModel model);
       Task<IEnumerable<GetByOwnerIdViewModel>> GetByOwnerIdProductsAsync(string id);
       Task UpdateDynamic<TViewModel>(TViewModel model) where TViewModel : UpdateProductViewModel;
    }
}

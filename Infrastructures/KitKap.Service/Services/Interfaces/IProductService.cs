using Kitkap.Service.Dtos.AddressDtos;
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
       Task<IEnumerable<RequestProductDto>> GetAllProducts();
       Task<GetByIdProductDto> GetByIdProduct(long id);
       Task AddAsync(CreateProductDto model);
       Task DeleteAsync(RemoveProductDto model);
       Task UpdateAsync(UpdateProductDto model);
       Task<IEnumerable<GetByOwnerIdDto>> GetByOwnerIdProductsAsync(string id);
       Task UpdateDynamic<TViewModel>(TViewModel model) where TViewModel : UpdateProductDto;
    }
}

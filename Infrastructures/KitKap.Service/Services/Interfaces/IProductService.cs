using Kitkap.Service.Dtos.AddressDtos;
using KitKap.Service.Dtos.ProductDtos;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kitkap.Service.Services
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

        Task<ProductFilterResultDto> GetFilteredProductsAsync(ProductFilterDto filters);
        Task<decimal> GetMinPriceAsync();
        Task<decimal> GetMaxPriceAsync();
        Task<(decimal Min, decimal Max)> GetPriceRangeAsync();
    }
}

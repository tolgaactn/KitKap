using AutoMapper;
using Kitkap.Entity.Entities;
using Kitkap.Entity.Services;
using Kitkap.Entity.UnitOfWorks;
using Kitkap.Service.Dtos.AddressDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KitKap.Service.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public ProductService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task AddAsync(CreateProductDto model)
        {
            await _uow.GetRepository<Product>().CreateAsync(_mapper.Map<Product>(model));
            await _uow.CommitAsync();
        }

        public async Task DeleteAsync(RemoveProductDto model)
        {
            var Product = await _uow.GetRepository<Product>().GetByIdAsync(model.Id);

            if (Product == null)
                throw new KeyNotFoundException("Kitap bulunamadı");

            await _uow.GetRepository<Product>().DeleteAsync(Product);
        }

        public async Task<IEnumerable<RequestProductDto>> GetAllProducts()
        {
            var list = await _uow.GetRepository<Product>().GetAllAsync();
            return _mapper.Map<List<RequestProductDto>>(list);
        }

        public async Task<GetByIdProductDto> GetByIdProduct(long id)
        {
            var Product = await _uow.GetRepository<Product>().GetByIdAsync(id);
            return _mapper.Map<GetByIdProductDto>(Product);
            
        }

        public async Task<IEnumerable<GetByOwnerIdDto>> GetByOwnerIdProductsAsync(string id)
        {
            var Products = await _uow.GetRepository<Product>().GetAll(b => b.OwnerId == id);
            return _mapper.Map<List<GetByOwnerIdDto>>(Products);
        }

        public async Task UpdateAsync(UpdateProductDto model)
        {
            var product = await _uow.GetRepository<Product>().GetByIdAsync(model.Id);

            if (product == null)
                throw new KeyNotFoundException("Kitap bulunamadı ");

			if (product is Book book)
			{
				_mapper.Map(model, book);
			}
            
			product.Name = model.Name;
            product.Description = model.Description;
            product.Price = model.Price;
            product.Stock = model.Stock;
            product.IsAvailable = model.IsAvailable;
            product.OwnerId = model.OwnerId;
            product.CategoryId = model.CategoryId;
            product.IsDeleted = model.IsDeleted;
            
            
            

            await _uow.GetRepository<Product>().UpdateAsync(product);

            await _uow.CommitAsync();
        }

		public async Task UpdateDynamic<TViewModel>(TViewModel model) where TViewModel : UpdateProductDto
		{
            var product = await _uow.GetRepository<Product>().GetByIdAsync(model.Id);

			if (product == null)
				throw new KeyNotFoundException("Ürün bulunamadı");

			switch (product)
			{
				case Book book:
					var bookModel = model as UpdateBookDto;
					if (bookModel != null)
						_mapper.Map(bookModel, book);
					break;

				default:
					_mapper.Map(model, product);
					break;
			}

			await _uow.GetRepository<Product>().UpdateAsync(product);
			await _uow.CommitAsync();
		}
	}
}

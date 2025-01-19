using AutoMapper;
using Kitkap.Entity.Entities;
using Kitkap.Entity.Services;
using Kitkap.Entity.UnitOfWorks;
using Kitkap.Entity.ViewModels.BookViewModels;
using Kitkap.Entity.ViewModels.ProductViewModels.OtherTypesViewModels;
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

        public async Task AddAsync(CreateProductViewModel model)
        {
            await _uow.GetRepository<Product>().CreateAsync(_mapper.Map<Product>(model));
            await _uow.CommitAsync();
        }

        public async Task DeleteAsync(RemoveProductViewModel model)
        {
            var Product = await _uow.GetRepository<Product>().GetByIdAsync(model.Id);

            if (Product == null)
                throw new KeyNotFoundException("Kitap bulunamadı");

            await _uow.GetRepository<Product>().DeleteAsync(Product);
        }

        public async Task<IEnumerable<RequestProductViewModel>> GetAllProducts()
        {
            var list = await _uow.GetRepository<Product>().GetAllAsync();
            return _mapper.Map<List<RequestProductViewModel>>(list);
        }

        public async Task<GetByIdProductViewModel> GetByIdProduct(long id)
        {
            var Product = await _uow.GetRepository<Product>().GetByIdAsync(id);
            return _mapper.Map<GetByIdProductViewModel>(Product);
            
        }

        public async Task<IEnumerable<GetByOwnerIdViewModel>> GetByOwnerIdProductsAsync(string id)
        {
            var Products = await _uow.GetRepository<Product>().GetAll(b => b.OwnerId == id);
            return _mapper.Map<List<GetByOwnerIdViewModel>>(Products);
        }

        public async Task UpdateAsync(UpdateProductViewModel model)
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

		public async Task UpdateDynamic<TViewModel>(TViewModel model) where TViewModel : UpdateProductViewModel
		{
            var product = await _uow.GetRepository<Product>().GetByIdAsync(model.Id);

			if (product == null)
				throw new KeyNotFoundException("Ürün bulunamadı");

			switch (product)
			{
				case Book book:
					var bookModel = model as UpdateBookViewModel;
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

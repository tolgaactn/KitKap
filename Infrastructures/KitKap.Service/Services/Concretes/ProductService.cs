using AutoMapper;
using Kitkap.Entity.Entities;
using Kitkap.Entity.Services;
using Kitkap.Entity.UnitOfWorks;
using Kitkap.Service.Dtos.AddressDtos;
using Kitkap.Service.Services;
using KitKap.Service.Dtos.ProductDtos;
using KitKap.Service.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using static Kitkap.Entity.Entities.Product;

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
            var product = await _uow.GetRepository<Product>().GetByIdAsync(model.Id);

            if (product == null)
                throw new KeyNotFoundException("Kitap bulunamadı");

            product.IsDeleted = true;

            await _uow.GetRepository<Product>().UpdateAsync(product);

            await _uow.CommitAsync();
        }

        public async Task<IEnumerable<RequestProductDto>> GetAllProducts()
        {
            var list = await _uow.GetRepository<Product>().GetAll(
          includes: new Expression<Func<Product, object>>[]
        {
            p => p.ProductImages,
            p => p.Category
        });

            return _mapper.Map<List<RequestProductDto>>(list);
        }

        public async Task<GetByIdProductDto> GetByIdProduct(long id)
        {
            var Product = await _uow.GetRepository<Product>().GetByIdAsync(filter: x => x.Id == id, includes: new Expression<Func<Product, object>>[]
                 {
                c => c.ProductImages,
                c => c.Category,
                });
            return _mapper.Map<GetByIdProductDto>(Product);

        }



        public async Task<IEnumerable<GetByOwnerIdDto>> GetByOwnerIdProductsAsync(string id)
        {
            var Products = await _uow.GetRepository<Product>().GetAll(b => b.OwnerId == id);
            return _mapper.Map<List<GetByOwnerIdDto>>(Products);
        }

        // ✅ FİLTRELENMİŞ ÜRÜNLER
        public async Task<ProductFilterResultDto> GetFilteredProductsAsync(ProductFilterDto filters)
        {
            // 1. Base query
            var query = _uow.GetRepository<Product>()
                .GetQueryable()
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .Where(p => !p.IsDeleted && p.ApprovalStatus == ProductApprovalStatus.Approved);

            // 2. Kategori filtresi
            if (filters.CategoryId.HasValue && filters.CategoryId.Value > 0)
            {
                query = query.Where(p => p.CategoryId == filters.CategoryId.Value);
            }

            // 3. Fiyat filtresi
            if (filters.MinPrice.HasValue)
            {
                query = query.Where(p => p.Price >= filters.MinPrice.Value);
            }
            if (filters.MaxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= filters.MaxPrice.Value);
            }

            // 4. Durum filtresi (Yeni/İkinci El)
            if (!string.IsNullOrEmpty(filters.Condition))
            {
                if (filters.Condition.Equals("New", StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Where(p => p.Condition == ProductCondition.New);
                }
                else if (filters.Condition.Equals("Used", StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Where(p => p.Condition != ProductCondition.New);
                }
            }

            // 5. Stokta var filtresi
            if (filters.InStockOnly == true)
            {
                query = query.Where(p => p.Stock > 0 && p.Status == ProductStatus.InStock);
            }

            // 6. Arama
            if (!string.IsNullOrEmpty(filters.SearchQuery))
            {
                var searchLower = filters.SearchQuery.ToLower();
                query = query.Where(p =>
                    p.Name.ToLower().Contains(searchLower) ||
                    (p.Description != null && p.Description.ToLower().Contains(searchLower)) ||
                    p.Category.Name.ToLower().Contains(searchLower)
                );
            }

            // 7. Toplam ürün sayısı (filtrelenmiş)
            var totalProducts = await query.CountAsync();

            // 8. Sıralama
            query = filters.SortBy?.ToLower() switch
            {
                "price-asc" => query.OrderBy(p => p.Price),
                "price-desc" => query.OrderByDescending(p => p.Price),
                "newest" => query.OrderByDescending(p => p.CreatedAt),
                "name-asc" => query.OrderBy(p => p.Name),
                "name-desc" => query.OrderByDescending(p => p.Name),
                _ => query.OrderByDescending(p => p.CreatedAt)
            };

            // 9. Sayfalama
            var pagedProducts = await query
                .Skip((filters.PageNumber - 1) * filters.PageSize)
                .Take(filters.PageSize)
                .AsNoTracking()
                .ToListAsync();

            // 10. DTO'ya map et
            var productDtos = _mapper.Map<List<RequestProductDto>>(pagedProducts);

            // 11. Min-Max fiyat (Tek sorguda)
            var priceRange = await GetPriceRangeAsync();

            // 12. Sonuç DTO'su
            return new ProductFilterResultDto
            {
                Products = productDtos,
                TotalProducts = totalProducts,
                TotalPages = (int)Math.Ceiling(totalProducts / (double)filters.PageSize),
                CurrentPage = filters.PageNumber,
                PageSize = filters.PageSize,
                MinAvailablePrice = priceRange.Min,
                MaxAvailablePrice = priceRange.Max
            };
        }



        // ✅ MINIMUM FİYAT
        public async Task<decimal> GetMinPriceAsync()
        {
            var prices = await _uow.GetRepository<Product>()
                .GetQueryable()
                .Where(p => !p.IsDeleted && p.ApprovalStatus == ProductApprovalStatus.Approved)
                .Select(p => p.Price)
                .ToListAsync();

            return prices.Any() ? prices.Min() : 0;
        }

        // ✅ MAXIMUM FİYAT
        public async Task<decimal> GetMaxPriceAsync()
        {
            var prices = await _uow.GetRepository<Product>()
                .GetQueryable()
                .Where(p => !p.IsDeleted && p.ApprovalStatus == ProductApprovalStatus.Approved)
                .Select(p => p.Price)
                .ToListAsync();

            return prices.Any() ? prices.Max() : 1000;
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
            product.Status = model.Status;
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
        // ✅ FİYAT ARALIĞI (Min ve Max birlikte - PERFORMANSLI)
        public async Task<(decimal Min, decimal Max)> GetPriceRangeAsync()
        {
            var prices = await _uow.GetRepository<Product>()
                .GetQueryable()
                .Where(p => !p.IsDeleted && p.ApprovalStatus == ProductApprovalStatus.Approved)
                .Select(p => p.Price)
                .ToListAsync();

            if (!prices.Any())
            {
                return (Min: 0, Max: 1000);
            }

            return (Min: prices.Min(), Max: prices.Max());
        }
    }
}

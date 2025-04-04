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
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public CategoryService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task AddAsync(CreateCategoryDto model)
        {
            await _uow.GetRepository<Category>().CreateAsync(_mapper.Map<Category>(model)); 
            await _uow.CommitAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var category = await _uow.GetRepository<Category>().GetByIdAsync(id);

            if (category == null)
                throw new KeyNotFoundException("Bu id'de kategori bulunamadı");

            category.IsDeleted= true;

            await _uow.GetRepository<Category>().UpdateAsync(category);

            await _uow.CommitAsync();
        }

        public async Task<IEnumerable<RequestCategoryDto>> GetAllCategories()
        {
            var list = await _uow.GetRepository<Category>().GetAllAsync();
            return _mapper.Map<List<RequestCategoryDto>>(list);
        }

        public async Task<GetByIdCategoryDto> GetByIdCategory(int id)
        {
            var category = await _uow.GetRepository<Category>().GetByIdAsync(id);
            return _mapper.Map<GetByIdCategoryDto>(category);
        }

        public async Task UpdateAsync(UpdateCategoryDto model)
        {
            var category = await _uow.GetRepository<Category>().GetByIdAsync(model.Id);

            if (category == null)
                throw new KeyNotFoundException("Kategori bulunamadı ");

            category.Name = model.Name;
            category.Description = model.Description;
            category.IsDeleted = model.IsDeleted;
            category.ParentCategoryId = model.ParentCategoryId;
            
            await _uow.GetRepository<Category>().UpdateAsync(category);

            await _uow.CommitAsync();
        }
    }
}

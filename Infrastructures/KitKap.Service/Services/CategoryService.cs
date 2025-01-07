using AutoMapper;
using Kitkap.Entity.Entities;
using Kitkap.Entity.Services;
using Kitkap.Entity.UnitOfWorks;
using Kitkap.Entity.ViewModels.CategoryViewModels;
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

        public async Task AddAsync(CreateCategoryViewModel model)
        {
            await _uow.GetRepository<Category>().CreateAsync(_mapper.Map<Category>(model)); 
            await _uow.CommitAsync();
        }

        public async Task DeleteAsync(RemoveCategoryViewModel model)
        {
            var category = await _uow.GetRepository<Category>().GetByIdAsync(model.CategoryId);

            if (category == null)
                throw new KeyNotFoundException("Bu id'de kategori bulunamadı");

            await _uow.GetRepository<Category>().DeleteAsync(category);
        }

        public async Task<IEnumerable<RequestCategoryViewModel>> GetAllCategories()
        {
            var list = await _uow.GetRepository<Category>().GetAllAsync();
            return _mapper.Map<List<RequestCategoryViewModel>>(list);
        }

        public async Task<GetByIdCategoryViewModel> GetByIdCategory(int id)
        {
            var category = await _uow.GetRepository<Category>().GetByIdAsync(id);
            return _mapper.Map<GetByIdCategoryViewModel>(category);
        }

        public async Task UpdateAsync(UpdateCategoryViewModel model)
        {
            var category = await _uow.GetRepository<Category>().GetByIdAsync(model.CategoryId);

            if (category == null)
                throw new KeyNotFoundException("Kategori bulunamadı ");

            category.Name = model.Name;
            
            await _uow.GetRepository<Category>().UpdateAsync(category);

            await _uow.CommitAsync();
        }
    }
}

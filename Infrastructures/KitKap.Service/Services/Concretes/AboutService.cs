using AutoMapper;
using Kitkap.Entity.Entities;
using Kitkap.Entity.Services;
using Kitkap.Entity.UnitOfWorks;
using Kitkap.Service.Dtos.AddressDtos;
using KitKap.Service.Dtos.AboutDtos;
using KitKap.Service.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KitKap.Service.Services.Concretes
{
        public class AboutService : IAboutService
        {
            private readonly IUnitOfWork _uow;
            private readonly IMapper _mapper;

            public AboutService(IUnitOfWork uow, IMapper mapper)
            {
                _uow = uow;
                _mapper = mapper;
            }

            public async Task CreateAboutAsync(CreateAboutDto createAboutDto)
            {
                await _uow.GetRepository<About>().CreateAsync(_mapper.Map<About>(createAboutDto));
                await _uow.CommitAsync();
            }

            public async Task DeleteAboutAsync(int id)
            {
                var About = await _uow.GetRepository<About>().GetByIdAsync(id);

                if (About == null)
                    throw new KeyNotFoundException("Bu id'de hakkında bulunamadı");

                //About.IsDeleted = true;

                await _uow.GetRepository<About>().UpdateAsync(About);

                await _uow.CommitAsync();
            }

            public async Task<IEnumerable<ResultAboutDto>> GetAllAboutAsync()
            {
                var list = await _uow.GetRepository<About>().GetAllAsync();
                return _mapper.Map<List<ResultAboutDto>>(list);
            }

            public async Task<GetByIdAboutDto> GetByIdAboutAsync(int id)
            {
                var About = await _uow.GetRepository<About>().GetByIdAsync(id);
                return _mapper.Map<GetByIdAboutDto>(About);
            }

            public async Task UpdateAboutAsync(UpdateAboutDto updateAboutDto)
            {
                var About = await _uow.GetRepository<About>().GetByIdAsync(updateAboutDto.AboutId);

                if (About == null)
                    throw new KeyNotFoundException("Hakkında bulunamadı ");

                About.Address = About.Address;
                About.Description = About.Description;
                About.Email = About.Email;
                About.Phone = About.Phone;

                await _uow.GetRepository<About>().UpdateAsync(About);

                await _uow.CommitAsync();
            }
        }
}

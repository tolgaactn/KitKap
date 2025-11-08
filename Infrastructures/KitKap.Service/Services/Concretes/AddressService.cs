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

namespace KitKap.Service.Services.Concretes
{
    public class AddressService : IAddressService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public AddressService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task AddAsync(CreateAddressDto model)
        {
            await _uow.GetRepository<Address>().CreateAsync(_mapper.Map<Address>(model));
            await _uow.CommitAsync();
        }

        public async Task DeleteAsync(RemoveAddressDto model)
        {
            var address = await _uow.GetRepository<Address>().GetByIdAsync(model.Id);

            if (address == null)
                throw new KeyNotFoundException("Address bulunamadı");

            await _uow.GetRepository<Address>().DeleteAsync(address);
        }

        public async Task<IEnumerable<RequestAddressDto>> GetAllAddresses()
        {
            var list = await _uow.GetRepository<Address>().GetAllAsync();
            return _mapper.Map<List<RequestAddressDto>>(list);
        }

        public async Task<GetByIdAddressDto> GetByIdAddress(int id)
        {
            var address = await _uow.GetRepository<Address>().GetByIdAsync(id);
            return _mapper.Map<GetByIdAddressDto>(address);
        }

        /// <summary>
        /// Kullanıcının tüm adreslerini getirir
        /// </summary>
        public async Task<IEnumerable<RequestAddressDto>> GetByUserIdAsync(string userId)
        {
            var addresses = await _uow.GetRepository<Address>().GetAll(
                filter: a => a.UserId == userId && !a.IsDeleted,
                orderby: q => q.OrderByDescending(a => a.Id) // En yeni adres önce
            );

            return _mapper.Map<List<RequestAddressDto>>(addresses);
        }

        public async Task UpdateAsync(UpdateAddressDto model)
        {
            var address = await _uow.GetRepository<Address>().GetByIdAsync(model.Id);

            if (address == null)
                throw new KeyNotFoundException("Adres bulunamadı");

            address.City = model.City;
            address.Country = model.Country;
            address.PostCode = model.PostCode;
            address.UserId = model.UserId;
            address.District = model.District;
            address.Description = model.Description;

            await _uow.GetRepository<Address>().UpdateAsync(address);

            await _uow.CommitAsync();
        }
    }
}

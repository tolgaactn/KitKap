using AutoMapper;
using Kitkap.Entity.Entities;
using Kitkap.Entity.Services;
using Kitkap.Entity.UnitOfWorks;
using Kitkap.Entity.ViewModels.AddressViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KitKap.Service.Services
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

        public async Task AddAsync(CreateAddressViewModel model)
        {
            await _uow.GetRepository<Address>().CreateAsync(_mapper.Map<Address>(model));
            await _uow.CommitAsync();
        }

        public async Task DeleteAsync(RemoveAddressViewModel model)
        {
            var address = await _uow.GetRepository<Address>().GetByIdAsync(model.AddressId);

            if (address == null)
                throw new KeyNotFoundException("Address bulunamadı");

            await _uow.GetRepository<Address>().DeleteAsync(address);
        }

        public async Task<IEnumerable<RequestAddressViewModel>> GetAllAddresses()
        {
            var list = await _uow.GetRepository<Address>().GetAllAsync();
            return _mapper.Map<List<RequestAddressViewModel>>(list);
        }

        public async Task<GetByIdAddressViewModel> GetByIdAddress(int id)
        {
            var address = await _uow.GetRepository<Address>().GetByIdAsync(id);
            return _mapper.Map<GetByIdAddressViewModel>(address);
        }

        public async Task UpdateAsync(UpdateAddressViewModel model)
        {
            var address = await _uow.GetRepository<Address>().GetByIdAsync(model.AddressId);

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

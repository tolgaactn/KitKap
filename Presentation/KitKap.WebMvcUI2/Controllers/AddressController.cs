using Kitkap.Entity.Services;
using Kitkap.Entity.ViewModels.AddressViewModels;
using Kitkap.Entity.ViewModels.BookViewModels;
using KitKap.Service.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kitkap.WebMvcUI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        private readonly IAddressService _addressService;

        public AddressController(IAddressService addressService)
        {
            _addressService = addressService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAddresses()
        {
            var addresses = await _addressService.GetAllAddresses();
            return Ok(addresses);
        }

        [HttpGet("id")]
        public async Task<IActionResult> GetAddressById(int id)
        {
            var address = await _addressService.GetByIdAddress(id);
            return Ok(address);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAddress(CreateAddressViewModel model)
        {
            await _addressService.AddAsync(model);
            return Ok(model.City);
        }
        [HttpPut]
        public async Task<IActionResult> UpdateAddress(UpdateAddressViewModel model)
        {
            await _addressService.UpdateAsync(model);
            return Ok(model.City);
        }
        [HttpDelete]
        public async Task<IActionResult> RemoveAddress(RemoveAddressViewModel model)
        {
            await _addressService.DeleteAsync(model);
            return Ok("Adres Güncellendi");
        }
    }
}

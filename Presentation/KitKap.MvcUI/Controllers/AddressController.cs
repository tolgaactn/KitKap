using AutoMapper;
using Kitkap.Entity.Services;
using Kitkap.Service.Dtos.AddressDtos;
using KitKap.MvcUI.ViewModels.AddressViewModels;
using KitKap.Service.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace KitKap.MvcUI.Controllers
{
    [Authorize]
    public class AddressController : Controller
    {
        private readonly IAddressService _addressService;
        private readonly IMapper _mapper;

        public AddressController(IAddressService addressService, IMapper mapper)
        {
            _addressService = addressService;
            _mapper = mapper;
        }

        // ========================================
        // ➕ YENİ ADRES EKLE
        // ========================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateAddressViewModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Lütfen tüm alanları doğru şekilde doldurunuz.";
                TempData["ActiveTab"] = "addresses-tab"; // ✅ Önemli
                return RedirectToAction("Profile", "Account"); // ✅ Account/Profile'a dön
            }

            try
            {
                var createAddressDto = new CreateAddressDto { City = model.City, Country = model.Country, Description = model.Description, District = model.District, PostCode = model.PostCode, UserId = userId };


                await _addressService.AddAsync(createAddressDto);

                TempData["Success"] = "Adres başarıyla eklendi! ✅";
                TempData["ActiveTab"] = "addresses-tab"; // ✅ Önemli
                return RedirectToAction("Profile", "Account"); // ✅ Account/Profile'a dön
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Adres eklenirken bir hata oluştu: " + ex.Message;
                TempData["ActiveTab"] = "addresses-tab"; // ✅ Önemli
                return RedirectToAction("Profile", "Account"); // ✅ Account/Profile'a dön
            }
        }

        // ========================================
        // ✏️ ADRES DÜZENLE
        // ========================================

        [HttpGet]
        public async Task<IActionResult> GetAddress(int id)
        {
            try
            {
                var address = await _addressService.GetByIdAddress(id);

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (address.UserId != userId)
                {
                    return Json(new { success = false, message = "Yetkisiz işlem!" });
                }

                return Json(new { success = true, data = address });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(UpdateAddressViewModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Lütfen tüm alanları doğru şekilde doldurunuz.";
                TempData["ActiveTab"] = "addresses-tab"; // ✅ Önemli
                return RedirectToAction("Profile", "Account"); // ✅ Account/Profile'a dön
            }

            try
            {
                var existingAddress = await _addressService.GetByIdAddress(model.Id);

                if (existingAddress.UserId != userId)
                {
                    TempData["Error"] = "Yetkisiz işlem!";
                    TempData["ActiveTab"] = "addresses-tab"; // ✅ Önemli
                    return RedirectToAction("Profile", "Account"); // ✅ Account/Profile'a dön
                }
                var updateAddressDto = new UpdateAddressDto { City = model.City, Country = model.Country, Description = model.Description, District = model.District, Id = model.Id, PostCode = model.PostCode, UserId = userId };
                await _addressService.UpdateAsync(updateAddressDto);

                TempData["Success"] = "Adres başarıyla güncellendi! ✅";
                TempData["ActiveTab"] = "addresses-tab"; // ✅ Önemli
                return RedirectToAction("Profile", "Account"); // ✅ Account/Profile'a dön
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Adres güncellenirken bir hata oluştu: " + ex.Message;
                TempData["ActiveTab"] = "addresses-tab"; // ✅ Önemli
                return RedirectToAction("Profile", "Account"); // ✅ Account/Profile'a dön
            }
        }

        // ========================================
        // 🗑️ ADRES SİL
        // ========================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var address = await _addressService.GetByIdAddress(id);

                if (address.UserId != userId)
                {
                    TempData["Error"] = "Yetkisiz işlem!";
                    TempData["ActiveTab"] = "addresses-tab"; // ✅ Önemli
                    return RedirectToAction("Profile", "Account"); // ✅ Account/Profile'a dön
                }

                var removeDto = new RemoveAddressDto { Id = id };
                await _addressService.DeleteAsync(removeDto);

                TempData["Success"] = "Adres başarıyla silindi! 🗑️";
                TempData["ActiveTab"] = "addresses-tab"; // ✅ Önemli
                return RedirectToAction("Profile", "Account"); // ✅ Account/Profile'a dön
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Adres silinirken bir hata oluştu: " + ex.Message;
                TempData["ActiveTab"] = "addresses-tab"; // ✅ Önemli
                return RedirectToAction("Profile", "Account"); // ✅ Account/Profile'a dön
            }
        }
    }
}
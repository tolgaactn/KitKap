using Kitkap.Entity.Services;
using Kitkap.Service.Dtos.AddressDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace KitKap.MvcUI.Controllers
{
    [Authorize]
    public class AddressController : Controller
    {
        private readonly IAddressService _addressService;

        public AddressController(IAddressService addressService)
        {
            _addressService = addressService;
        }

        // ========================================
        // 📍 ADRESLERİM SAYFASI
        // ========================================

        /// <summary>
        /// Kullanıcının tüm adreslerini listele
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var addresses = await _addressService.GetByUserIdAsync(userId);

            return View(addresses);
        }

        // ========================================
        // ➕ YENİ ADRES EKLE
        // ========================================

        /// <summary>
        /// Yeni adres ekle (AJAX POST)
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateAddressDto model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Lütfen tüm alanları doğru şekilde doldurunuz.";
                return RedirectToAction("Index");
            }

            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                model.UserId = userId;

                await _addressService.AddAsync(model);

                TempData["Success"] = "Adres başarıyla eklendi! ✅";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Adres eklenirken bir hata oluştu: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        // ========================================
        // ✏️ ADRES DÜZENLE
        // ========================================

        /// <summary>
        /// Adres detayını getir (AJAX)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAddress(int id)
        {
            try
            {
                var address = await _addressService.GetByIdAddress(id);

                // Güvenlik kontrolü: Bu adres kullanıcıya ait mi?
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

        /// <summary>
        /// Adresi güncelle (POST)
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(UpdateAddressDto model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Lütfen tüm alanları doğru şekilde doldurunuz.";
                return RedirectToAction("Index");
            }

            try
            {
                // Güvenlik kontrolü
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var existingAddress = await _addressService.GetByIdAddress(model.Id);

                if (existingAddress.UserId != userId)
                {
                    TempData["Error"] = "Yetkisiz işlem!";
                    return RedirectToAction("Index");
                }

                await _addressService.UpdateAsync(model);

                TempData["Success"] = "Adres başarıyla güncellendi! ✅";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Adres güncellenirken bir hata oluştu: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        // ========================================
        // 🗑️ ADRES SİL
        // ========================================

        /// <summary>
        /// Adresi sil (POST)
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                // Güvenlik kontrolü
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var address = await _addressService.GetByIdAddress(id);

                if (address.UserId != userId)
                {
                    TempData["Error"] = "Yetkisiz işlem!";
                    return RedirectToAction("Index");
                }

                var removeDto = new RemoveAddressDto { Id = id };
                await _addressService.DeleteAsync(removeDto);

                TempData["Success"] = "Adres başarıyla silindi! 🗑️";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Adres silinirken bir hata oluştu: " + ex.Message;
                return RedirectToAction("Index");
            }
        }
    }

}

using Humanizer;
using KitKap.MvcUI.Areas.Admin.ViewModels.AboutViewModels;
using KitKap.Service.Dtos.AboutDtos;
using KitKap.Service.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KitKap.MvcUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AllowAnonymous]
    [Route("Admin/[controller]/[action]/{id?}")]
    public class AboutController : Controller
    {
            private readonly IAboutService _aboutService;

            public AboutController(IAboutService aboutService)
            {
                _aboutService = aboutService;
            }

            [HttpGet]
            public async Task<IActionResult> Index()
            {
                ViewBag.v1 = "Ana Sayfa";
                ViewBag.v2 = "Kategoriler";
                ViewBag.v3 = "Tüm Kategoriler";
                ViewBag.v0 = "Kategori İşlemleri";

                var AboutDtos = await _aboutService.GetAllAboutAsync();

                var viewModels = AboutDtos.Select(dto => new AboutViewModel
                {
                    AboutId = dto.AboutId,
                    Description = dto.Description,
                    Address = dto.Address,
                    Email = dto.Email,
                    Phone = dto.Phone
                }).ToList();

                return View(viewModels);
            }
            [HttpGet]
            public async Task<IActionResult> Create()
            {
                var categories = await _aboutService.GetAllAboutAsync();
                ViewBag.Categories = categories;
                return View();
            }

            [HttpPost]
            public async Task<IActionResult> Create(CreateAboutViewModel model)
            {

                if (ModelState.IsValid)
                {
                    var about = new CreateAboutDto
                    {
                        Description = model.Description,
                        Address = model.Address,
                        Email = model.Email,
                        Phone = model.Phone
                    };
                    await _aboutService.CreateAboutAsync(about);

                    return RedirectToAction("Index");
                }

                var categories = await _aboutService.GetAllAboutAsync();
                ViewBag.Categories = categories;

                return View(model);
            }

            [HttpPost]
            public async Task<IActionResult> Delete(int id)
            {
                await _aboutService.DeleteAboutAsync(id);

                return RedirectToAction("Index");
            }

            [HttpGet]
            public async Task<IActionResult> Edit(int id)
            {
                var AboutDto = await _aboutService.GetByIdAboutAsync(id);

                if (AboutDto == null)
                {
                    return NotFound();
                }

                // AboutDto'yu AboutViewModel'e dönüştürüyoruz
                var model = new AboutViewModel
                {
                    AboutId = AboutDto.AboutId,
                    Description = AboutDto.Description,
                    Address = AboutDto.Address,
                    Email = AboutDto.Email,
                    Phone = AboutDto.Phone
                };

                return View(model); // Düzenleme sayfasını gösterir
            }

            // Update (Düzenlenmiş kategoriyi kaydeder)
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Edit(AboutViewModel model)
            {
                if (ModelState.IsValid)
                {

                    var existingAbout = new UpdateAboutDto
                    {
                        AboutId = model.AboutId,
                        Description = model.Description,
                        Address = model.Address,
                        Email = model.Email,
                        Phone = model.Phone
                    };


                    await _aboutService.UpdateAboutAsync(existingAbout); // Güncellemeyi servis üzerinden yap

                    return RedirectToAction("Index");
                }

                // Model valid değilse, tekrar edit sayfasına dön
                return View(model);
            }
        }
    }


using Humanizer;
using KitKap.MvcUI.Areas.Admin.ViewModels.AboutViewModels;
using KitKap.Service.Dtos.AboutDtos;
using KitKap.Service.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace KitKap.MvcUI.Areas.Admin.Controllers
{
    [Route("Admin/About")]
    public class AboutController : BaseAdminController
    {
        private readonly IAboutService _aboutService;

        public AboutController(IAboutService aboutService)
        {
            _aboutService = aboutService;
        }

        [HttpGet]
        [Route("")]
        [Route("Index")]
        public async Task<IActionResult> Index(string? search)
        {
            var AboutDtos = await _aboutService.GetAllAboutAsync();

            var viewModels = AboutDtos.Select(dto => new AboutViewModel
            {
                AboutId = dto.AboutId,
                Description = dto.Description,
                Address = dto.Address,
                Email = dto.Email,
                Phone = dto.Phone
            }).ToList();

            ViewData["TotalCount"] = viewModels.Count();

            if (!string.IsNullOrWhiteSpace(search))
            {
                viewModels = viewModels.Where(a =>
                    (a.Description != null && a.Description.ToLower().Contains(search.ToLower().Trim())) ||
                    (int.TryParse(search.Trim(), out int Id) && a.AboutId == Id)
                ).ToList();
            }

            return View(viewModels);
        }

        [HttpGet]
        [Route("Create")]
        public async Task<IActionResult> Create()
        {
            var abouts = await _aboutService.GetAllAboutAsync();
            ViewBag.Abouts = abouts;
            return View();
        }

        [HttpPost]
        [Route("Create")]
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
                SetSuccessMessage("Hakkımızda bilgisi başarıyla eklendi!");
                return RedirectToAction("Index");
            }

            var categories = await _aboutService.GetAllAboutAsync();
            ViewBag.Categories = categories;

            return View(model);
        }

        [HttpPost]
        [Route("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _aboutService.DeleteAboutAsync(id);
            SetSuccessMessage("Hakkımızda bilgisi başarıyla silindi!");
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Route("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var AboutDto = await _aboutService.GetByIdAboutAsync(id);

            if (AboutDto == null)
            {
                return NotFound();
            }

            var model = new AboutViewModel
            {
                AboutId = AboutDto.AboutId,
                Description = AboutDto.Description,
                Address = AboutDto.Address,
                Email = AboutDto.Email,
                Phone = AboutDto.Phone
            };

            return View(model);
        }

        [HttpPost]
        [Route("Edit/{id}")]
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

                await _aboutService.UpdateAboutAsync(existingAbout);
                SetSuccessMessage("Hakkımızda bilgisi başarıyla güncellendi!");
                return RedirectToAction("Index");
            }

            return View(model);
        }
    }
}
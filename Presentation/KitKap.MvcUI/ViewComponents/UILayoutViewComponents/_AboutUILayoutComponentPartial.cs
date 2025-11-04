using KitKap.MvcUI.Areas.Admin.ViewModels.AboutViewModels;
using KitKap.Service.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace KitKap.MvcUI.ViewComponents.UILayoutViewComponents
{
    public class _AboutUILayoutComponentPartial : ViewComponent
    {
        private readonly IAboutService _aboutService;

        public _AboutUILayoutComponentPartial(IAboutService aboutService)
        {
            _aboutService = aboutService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var aboutDtos = await _aboutService.GetByIdAboutAsync(1);

            var viewModel = new AboutViewModel
            {
                Description = aboutDtos.Description,
                Address = aboutDtos.Address,
                Email = aboutDtos.Email,
                Phone = aboutDtos.Phone
            };

            return View(viewModel);
        }
    }
}

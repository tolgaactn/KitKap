using Humanizer;
using Kitkap.Entity.Entities;
using Kitkap.Service.Services;
using KitKap.MvcUI.Areas.Admin.ViewModels.AboutViewModels;
using KitKap.MvcUI.ViewModels.ProductDetailViewModels;
using KitKap.MvcUI.ViewModels.ShoppingCartDetailViewModels;
using KitKap.Service.Dtos.ShoppingCartDetailDtos;
using KitKap.Service.Extensions;
using KitKap.Service.Services.Concretes;
using KitKap.Service.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace KitKap.MvcUI.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly IProductService _productService;
        private readonly IShoppingCartDetailService _shoppingCartDetailService;
        private readonly IAboutService _aboutService;
        private readonly IProductImageService _productImageService;

        public ShoppingCartController(IProductService productService, IShoppingCartDetailService shoppingCartDetailService, IAboutService aboutService, IProductImageService productImageService)
        {
            _productService = productService;
            _shoppingCartDetailService = shoppingCartDetailService;
            _aboutService = aboutService;
            _productImageService = productImageService;
        }

        public async Task<IActionResult> Index()
        {           

            var shoppingCartDto = GetShoppingCart();

            var shoppingCartViewModel = new List<ShoppingCartDetailViewModel>();

            foreach (var dto in shoppingCartDto)
            {
                var productImages = await _productImageService.GetByIdProductImagesAsync(dto.productId);
                var imageUrls = productImages.Where(img => img.IsMain).Select(img => img.ImageUrl).ToList();

                shoppingCartViewModel.Add(new ShoppingCartDetailViewModel
                {
                    productId = dto.productId,
                    productName = dto.productName,
                    productQuantity = dto.productQuantity,
                    productPrice = dto.productPrice,
                    ImageUrls = imageUrls
                });
            }

            TempData["TotalQuantity"] =  _shoppingCartDetailService.TotalQuantity(shoppingCartDto);

            //if (_shoppingCartDetailService.TotalPrice(shoppingCartDto) > 0)
                TempData["TotalPrice"] = _shoppingCartDetailService.TotalPrice(shoppingCartDto);

            var aboutDtos = await _aboutService.GetAllAboutAsync();

            var viewModel = aboutDtos.Select(aboutDto => new AboutViewModel
            {
                AboutId = aboutDto.AboutId,
                Description = aboutDto.Description,
                Address = aboutDto.Address,
                Email = aboutDto.Email,
                Phone = aboutDto.Phone
            }).ToList();

            ViewBag.About = viewModel;

            return View(shoppingCartViewModel);
        }
        public async Task<IActionResult> Add(int id, int Adet)
        {
            var product = await _productService.GetByIdProduct(id);

            var shoppingCartDto = GetShoppingCart();

            ResultShoppingCartDetailDto orderDto = new ResultShoppingCartDetailDto();
            orderDto.productId = (int)product.Id;
            orderDto.productName = product.Name;
            orderDto.productQuantity = Adet;
            orderDto.productPrice = product.Price;

            shoppingCartDto = _shoppingCartDetailService.AddShoppingCart(shoppingCartDto, orderDto);
            SaveShoppingCart(shoppingCartDto);
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> AddStaySameAction(int Id, int Adet)
        {
            var product = await _productService.GetByIdProduct(Id);

            var shoppingCartDto = GetShoppingCart();

            ResultShoppingCartDetailDto orderDto = new ResultShoppingCartDetailDto();
            orderDto.productId = (int)product.Id;
            orderDto.productName = product.Name;
            orderDto.productQuantity = Adet;
            orderDto.productPrice = product.Price;

            shoppingCartDto = _shoppingCartDetailService.AddShoppingCart(shoppingCartDto, orderDto);
            SaveShoppingCart(shoppingCartDto);
            TempData["AddedToCart"] = true;
            return RedirectToAction("ProductDetail", "ProductList", new {id = product.Id});
        }
        public IActionResult Delete(int id)
        {
            var shoppingCartDto = GetShoppingCart();

            shoppingCartDto = _shoppingCartDetailService.DeleteFromShoppingCart(shoppingCartDto, id);
            SaveShoppingCart(shoppingCartDto);
            return RedirectToAction("Index");
        }
        public IActionResult DeleteFromDropdownCart(int id)
        {
            var shoppingCartDto = GetShoppingCart();

            shoppingCartDto = _shoppingCartDetailService.DeleteFromShoppingCart(shoppingCartDto, id);
            SaveShoppingCart(shoppingCartDto);
            // Geldiği sayfaya geri gönder
            string referer = Request.Headers["Referer"].ToString();
            if (!string.IsNullOrEmpty(referer))
            {
                return Redirect(referer);
            }

            return RedirectToAction("Index");
        }
        public IActionResult DeleteShoppingCart()
        {
            //HttpContext.Session.Clear(); //Oturumda bulunan tüm session'ları siler.
            HttpContext.Session.Remove("shoppingCart"); //Sadece ismi belirtilen session'ı siler.
            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult UpdateQuantity(int productId, int quantity)
        {

            var shoppingCartDto = GetShoppingCart();
            shoppingCartDto = _shoppingCartDetailService.UpdateQuantity(shoppingCartDto, productId, quantity);
            SaveShoppingCart(shoppingCartDto);

            var updatedItem = shoppingCartDto.FirstOrDefault(x => x.productId == productId);
            decimal updatedSubtotal = updatedItem != null ? updatedItem.productPrice * updatedItem.productQuantity : 0;
            decimal totalPrice = _shoppingCartDetailService.TotalPrice(shoppingCartDto);
            int totalQuantity = _shoppingCartDetailService.TotalQuantity(shoppingCartDto);

            return Json(new
            {
                success = true,
                updatedSubtotal,
                totalPrice,
                totalQuantity,
                updatedQuantity = updatedItem?.productQuantity ?? 1 // <= minicart için
            });
        }

        public List<ResultShoppingCartDetailDto> GetShoppingCart()
        {
            var shoppingCart = HttpContext.Session.GetJson<List<ResultShoppingCartDetailDto>>("shoppingCart") ?? new List<ResultShoppingCartDetailDto>();

            return shoppingCart;
        }
        public void SaveShoppingCart(List<ResultShoppingCartDetailDto> shoppingCart)
        {
            HttpContext.Session.SetJson("shoppingCart", shoppingCart);
        }
    }
}

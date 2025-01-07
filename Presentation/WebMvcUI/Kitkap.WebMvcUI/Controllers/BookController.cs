using Kitkap.Entity.Services;
using Kitkap.Entity.ViewModels.BookViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kitkap.WebMvcUI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly IBookService _bookService;
        private readonly ICategoryService _categoryService;
        private readonly IAccountService _accountService;

        public BookController(IBookService bookService, ICategoryService categoryService, IAccountService accountService)
        {
            _bookService = bookService;
            _categoryService = categoryService;
            _accountService = accountService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBooks()
        {
            var books = await _bookService.GetAllBooks();
            return Ok(books);
        }
        
        [HttpGet("id")]
        public async Task<IActionResult> GetBookById(int id)
        {
            var book = await _bookService.GetByIdBook(id);
            return Ok(book);
        }

        [HttpPost]
        public async Task<IActionResult> CreateBook(CreateBookViewModel model)
        {
                await _bookService.AddAsync(model);
                return Ok(model.Title);
        }
        [HttpPut]
        public async Task<IActionResult> UpdateBook(UpdateBookViewModel model)
        {
            await _bookService.UpdateAsync(model);
            return Ok(model.Title);
        }
        [HttpDelete]
        public async Task<IActionResult> RemoveBook(RemoveBookViewModel model)
        {
            await _bookService.DeleteAsync(model);
            return Ok("Kitap Güncellendi");
        }
        [HttpGet("OwnerId")]
        public async Task<IActionResult> GetBookByOwnerId(string id)
        {
            var books = await _bookService.GetByOwnerIdBooksAsync(id);
            return Ok(books);
        }
    }
}

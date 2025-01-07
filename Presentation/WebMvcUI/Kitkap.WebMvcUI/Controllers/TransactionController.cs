using Kitkap.Entity.Services;
using Kitkap.Entity.ViewModels.TransactionViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kitkap.WebMvcUI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTransactions(RequestTransactionViewModel model)
        {
            var transactions = await _transactionService.GetAllTransactions();
            return Ok(transactions);
        }
        [HttpGet("id")]
        public async Task<IActionResult> GetTransactionById(GetByIdTransactionViewModel model)
        {
            var transaction = await _transactionService.GetByIdTransaction(model.TransactionId);
            return Ok(transaction);
        }
        [HttpPost]
        public async Task<IActionResult> CreateTransaction(CreateTransactionViewModel model)
        {
            await _transactionService.CreateAsync(model);
            return Ok("Transfer gerçekleşti"+model.BookId);
        }

    }
}

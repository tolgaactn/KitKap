using Kitkap.Entity.ViewModels.TransactionViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kitkap.Entity.Services
{
    public interface ITransactionService
    {
        Task<IEnumerable<RequestTransactionViewModel>> GetAllTransactions();
        Task<GetByIdTransactionViewModel> GetByIdTransaction(int id);
        Task CreateAsync(CreateTransactionViewModel model);
        Task DeleteAsync(RemoveTransactionViewModel model);
        Task UpdateAsync(UpdateTransactionViewModel model);
    }
}

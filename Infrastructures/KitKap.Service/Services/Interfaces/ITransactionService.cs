using Kitkap.Service.Dtos.AddressDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kitkap.Entity.Services
{
    public interface ITransactionService
    {
        Task<IEnumerable<RequestTransactionDto>> GetAllTransactions();
        Task<GetByIdTransactionDto> GetByIdTransaction(int id);
        Task CreateAsync(CreateTransactionDto model);
        Task DeleteAsync(RemoveTransactionDto model);
        Task UpdateAsync(UpdateTransactionDto model);
    }
}

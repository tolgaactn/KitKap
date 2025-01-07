using Kitkap.Entity.ViewModels.BookViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kitkap.Entity.Services
{
    public interface IBookService
    {
       Task<IEnumerable<RequestBookViewModel>> GetAllBooks();
       Task<GetByIdBookViewModel> GetByIdBook(int id);
       Task AddAsync(CreateBookViewModel model);
       Task DeleteAsync(RemoveBookViewModel model);
       Task UpdateAsync(UpdateBookViewModel model);
       Task<IEnumerable<GetByOwnerIdViewModel>> GetByOwnerIdBooksAsync(string id);
    }
}

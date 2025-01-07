using AutoMapper;
using Kitkap.Entity.Entities;
using Kitkap.Entity.Services;
using Kitkap.Entity.UnitOfWorks;
using Kitkap.Entity.ViewModels.TransactionViewModels;
using KitKap.DataAccess.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KitKap.Service.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public TransactionService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task CreateAsync(CreateTransactionViewModel model)
        {
            var sender = await _uow.GetRepository<AppUser>().GetByIdAsync(model.SenderId);
            var receiver = await _uow.GetRepository<AppUser>().GetByIdAsync(model.ReceiverId);

            if(sender == null || receiver == null) 
            {
                throw new Exception("Gönderici ya da kullanıcı bulunamadı");
            }

            var book = await _uow.GetRepository<Book>().GetByIdAsync(model.BookId);

            if(book.IsAvailable == false)
            {
                throw new Exception("Kitap satılmış");
            }

            if(book == null)
            {
                throw new Exception("Kitap bulunamadı");
            }
            if (book.BookPoint > receiver.Balance)
                throw new Exception("Alıcı bakiyesi yetersiz");

            var transaction = new Transaction
            {
                BookId = model.BookId,
                SenderId = model.SenderId,
                ReceiverId = model.ReceiverId,
                TransactionDate = model.TransactionDate,
                Status = "İşlem Başarılı, Kitap Gönderilmeli.",
                PointTransferred = book.BookPoint,
                
                
            };
            book.IsAvailable = false;

            sender.Balance += book.BookPoint;
            receiver.Balance -= book.BookPoint;

            await _uow.GetRepository<Transaction>().CreateAsync(transaction);

            await _uow.GetRepository<AppUser>().UpdateAsync(sender);
            await _uow.GetRepository<AppUser>().UpdateAsync(receiver);

            await _uow.CommitAsync();

        }

        public Task DeleteAsync(RemoveTransactionViewModel model)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<RequestTransactionViewModel>> GetAllTransactions()
        {
            throw new NotImplementedException();
        }

        public Task<GetByIdTransactionViewModel> GetByIdTransaction(int id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(UpdateTransactionViewModel model)
        {
            throw new NotImplementedException();
        }
    }
}

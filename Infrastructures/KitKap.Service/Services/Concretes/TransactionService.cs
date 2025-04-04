using AutoMapper;
using Kitkap.Entity.Entities;
using Kitkap.Entity.Services;
using Kitkap.Entity.UnitOfWorks;
using Kitkap.Service.Dtos.AddressDtos;
using KitKap.DataAccess.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KitKap.Service.Services.Concretes
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

        public async Task CreateAsync(CreateTransactionDto model)
        {
            var sender = await _uow.GetRepository<AppUser>().GetByIdAsync(model.SenderId);
            var receiver = await _uow.GetRepository<AppUser>().GetByIdAsync(model.ReceiverId);

            if (sender == null || receiver == null)
            {
                throw new Exception("Gönderici ya da kullanıcı bulunamadı");
            }

            var product = await _uow.GetRepository<Product>().GetByIdAsync(model.ProductId);

            if (product.IsAvailable == false)
            {
                throw new Exception("Ürün satılmış");
            }

            if (product == null)
            {
                throw new Exception("Kitap bulunamadı");
            }
            if (product.Price > receiver.Balance)
                throw new Exception("Alıcı bakiyesi yetersiz");

            var transaction = new Transaction
            {
                ProductId = model.ProductId,
                SenderId = model.SenderId,
                ReceiverId = model.ReceiverId,
                TransactionDate = model.TransactionDate,
                Status = "İşlem Başarılı, ürüm Gönderilmeli.",
                PointTransferred = product.Price,


            };
            product.IsAvailable = false;

            sender.Balance += product.Price;
            receiver.Balance -= product.Price;

            await _uow.GetRepository<Transaction>().CreateAsync(transaction);

            await _uow.GetRepository<AppUser>().UpdateAsync(sender);
            await _uow.GetRepository<AppUser>().UpdateAsync(receiver);

            await _uow.CommitAsync();

        }

        public Task DeleteAsync(RemoveTransactionDto model)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<RequestTransactionDto>> GetAllTransactions()
        {
            throw new NotImplementedException();
        }

        public Task<GetByIdTransactionDto> GetByIdTransaction(int id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(UpdateTransactionDto model)
        {
            throw new NotImplementedException();
        }
    }
}

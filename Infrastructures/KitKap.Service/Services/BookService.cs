using AutoMapper;
using Kitkap.Entity.Entities;
using Kitkap.Entity.Services;
using Kitkap.Entity.UnitOfWorks;
using Kitkap.Entity.ViewModels.BookViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KitKap.Service.Services
{
    public class BookService : IBookService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public BookService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task AddAsync(CreateBookViewModel model)
        {
            await _uow.GetRepository<Book>().CreateAsync(_mapper.Map<Book>(model));
            await _uow.CommitAsync();
        }

        public async Task DeleteAsync(RemoveBookViewModel model)
        {
            var book = await _uow.GetRepository<Book>().GetByIdAsync(model.Id);

            if (book == null)
                throw new KeyNotFoundException("Kitap bulunamadı");

            await _uow.GetRepository<Book>().DeleteAsync(book);
        }

        public async Task<IEnumerable<RequestBookViewModel>> GetAllBooks()
        {
            var list = await _uow.GetRepository<Book>().GetAllAsync();
            return _mapper.Map<List<RequestBookViewModel>>(list);
        }

        public async Task<GetByIdBookViewModel> GetByIdBook(int id)
        {
            var book = await _uow.GetRepository<Book>().GetByIdAsync(id);
            return _mapper.Map<GetByIdBookViewModel>(book);
            
        }

        public async Task<IEnumerable<GetByOwnerIdViewModel>> GetByOwnerIdBooksAsync(string id)
        {
            var books = await _uow.GetRepository<Book>().GetAll(b => b.OwnerId == id);
            return _mapper.Map<List<GetByOwnerIdViewModel>>(books);
        }

        public async Task UpdateAsync(UpdateBookViewModel model)
        {
            var book = await _uow.GetRepository<Book>().GetByIdAsync(model.BookId);

            if (book == null)
                throw new KeyNotFoundException("Kitap bulunamadı ");

            book.ISBN = model.ISBN;
            book.Title = model.Title;
            book.Author = model.Author;
            book.PublicationDate = model.PublicationDate;
            book.CategoryId = model.CategoryId;
            book.BookPoint = model.BookPoint;
            book.Language = model.Language;
            book.Condition = model.Condition;
            book.OwnerId = model.OwnerId;
            book.IsAvailable = model.IsAvailable;

            await _uow.GetRepository<Book>().UpdateAsync(book);

            await _uow.CommitAsync();
        }
    }
}

using AutoMapper;
using Kitkap.Entity.Entities;
using Kitkap.Entity.ViewModels.AddressViewModels;
using Kitkap.Entity.ViewModels.BookViewModels;
using Kitkap.Entity.ViewModels.CategoryViewModels;
using Kitkap.Entity.ViewModels.TransactionViewModels;
using Kitkap.Entity.ViewModels.UserViewModels;
using KitKap.DataAccess.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KitKap.Service.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile() 
        {
            CreateMap<Book, RequestBookViewModel>().ReverseMap();
            CreateMap<Book, CreateBookViewModel>().ReverseMap();
            CreateMap<Book, UpdateBookViewModel>().ReverseMap();
            CreateMap<Book, RemoveBookViewModel>().ReverseMap();
            CreateMap<Book, GetByIdBookViewModel>().ReverseMap();
            CreateMap<Book, GetByOwnerIdViewModel>().ReverseMap();

            CreateMap<Category, RequestCategoryViewModel>().ReverseMap();
            CreateMap<Category, CreateCategoryViewModel>().ReverseMap();
            CreateMap<Category, UpdateCategoryViewModel>().ReverseMap();
            CreateMap<Category, RemoveCategoryViewModel>().ReverseMap();
            CreateMap<Category, GetByIdCategoryViewModel>().ReverseMap();

            CreateMap<Transaction, RequestTransactionViewModel>().ReverseMap();
            CreateMap<Transaction, CreateTransactionViewModel>().ReverseMap();
            CreateMap<Transaction, UpdateTransactionViewModel>().ReverseMap();
            CreateMap<Transaction, RemoveTransactionViewModel>().ReverseMap();
            CreateMap<Transaction, GetByIdTransactionViewModel>().ReverseMap();

            CreateMap<Address, RequestAddressViewModel>().ReverseMap();
            CreateMap<Address, CreateAddressViewModel>().ReverseMap();
            CreateMap<Address, UpdateAddressViewModel>().ReverseMap();
            CreateMap<Address, RemoveAddressViewModel>().ReverseMap();
            CreateMap<Address, GetByIdAddressViewModel>().ReverseMap();

            CreateMap<AppUser, GetByIdUserViewModel>().ReverseMap();
            CreateMap<AppUser, LoginUserViewModel>().ReverseMap();
            CreateMap<AppUser, RegisterUserViewModel>().ReverseMap();
            CreateMap<AppUser, RequestUserViewModel>().ReverseMap();
        }
    }
}

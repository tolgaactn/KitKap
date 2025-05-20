using AutoMapper;
using Kitkap.Entity.Entities;
using KitKap.DataAccess.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kitkap.Service.Dtos.AddressDtos;
using KitKap.Service.Dtos.AboutDtos;
using KitKap.Service.Dtos.ProductImagesDtos;
using Kitkap.Service.Dtos.UserDtos;
using KitKap.Service.Dtos.ShoppingCartDetailDtos;

namespace KitKap.Service.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile() 
        {
            CreateMap<Product, RequestProductDto>().ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name));
            CreateMap<Product, CreateProductDto>().ReverseMap();
            CreateMap<Product, UpdateProductDto>().ReverseMap();
            CreateMap<Product, RemoveProductDto>().ReverseMap();
            CreateMap<Product, GetByIdProductDto>().Include<Book, GetByIdProductDto>().ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name));
            CreateMap<Book, GetByIdProductDto>();
            CreateMap<Product, GetByOwnerIdDto>().ReverseMap();

            CreateMap<ProductImage, CreateProductImageDto>().ReverseMap();
            CreateMap<ProductImage, RequestProductImageDto>().ReverseMap();

            CreateMap<Category, ResultCategoryDto>().ReverseMap();
            CreateMap<Category, CreateCategoryDto>().ReverseMap();
            CreateMap<Category, UpdateCategoryDto>().ReverseMap();
            CreateMap<Category, RemoveCategoryDto>().ReverseMap();
            CreateMap<Category, GetByIdCategoryDto>().ReverseMap();

            CreateMap<Transaction, RequestTransactionDto>().ReverseMap();
            CreateMap<Transaction, CreateTransactionDto>().ReverseMap();
            CreateMap<Transaction, UpdateTransactionDto>().ReverseMap();
            CreateMap<Transaction, RemoveTransactionDto>().ReverseMap();
            CreateMap<Transaction, GetByIdTransactionDto>().ReverseMap();

            CreateMap<Address, RequestAddressDto>().ReverseMap();
            CreateMap<Address, CreateAddressDto>().ReverseMap();
            CreateMap<Address, UpdateAddressDto>().ReverseMap();
            CreateMap<Address, RemoveAddressDto>().ReverseMap();
            CreateMap<Address, GetByIdAddressDto>().ReverseMap();

            CreateMap<AppUser, GetByIdUserDto>().ReverseMap();
            CreateMap<AppUser, LoginUserDto>().ReverseMap();
            CreateMap<AppUser, RegisterUserDto>().ReverseMap();
            CreateMap<AppUser, RequestUserDto>().ReverseMap();

            CreateMap<About, ResultAboutDto>().ReverseMap();
            CreateMap<About, CreateAboutDto>().ReverseMap();
            CreateMap<About, UpdateAboutDto>().ReverseMap();
            CreateMap<About, GetByIdAboutDto>().ReverseMap();

            CreateMap<Book, UpdateBookDto>().IncludeBase<Product,UpdateProductDto>().ReverseMap();

            CreateMap<ShoppingCartDetail, ResultShoppingCartDetailDto>().ReverseMap();



        }
    }
}

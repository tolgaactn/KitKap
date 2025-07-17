using Kitkap.Entity.Entities;
using KitKap.Service.Dtos.ShoppingCartDetailDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KitKap.Service.Services.Interfaces
{
    public interface IShoppingCartDetailService
    {
       List<ResultShoppingCartDetailDto> AddShoppingCart(List<ResultShoppingCartDetailDto> shoppingCart, ResultShoppingCartDetailDto order);
       List<ResultShoppingCartDetailDto> DeleteFromShoppingCart(List<ResultShoppingCartDetailDto> shoppingCart, int id);
       int TotalQuantity(List<ResultShoppingCartDetailDto> shoppingCart);
       decimal TotalPrice(List<ResultShoppingCartDetailDto> shoppingCart);
       List<ResultShoppingCartDetailDto> UpdateQuantity(List<ResultShoppingCartDetailDto> shoppingCart, int productId, int newQuantity);
    }
}

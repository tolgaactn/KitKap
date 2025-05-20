using AutoMapper;
using Kitkap.Entity.Entities;
using Kitkap.Entity.UnitOfWorks;
using KitKap.Service.Dtos.ShoppingCartDetailDtos;
using KitKap.Service.Services.Interfaces;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KitKap.Service.Services.Concretes
{
    public class ShoppingCartDetailService : IShoppingCartDetailService
    {
        public List<ResultShoppingCartDetailDto> AddShoppingCart(List<ResultShoppingCartDetailDto> shoppingCart, ResultShoppingCartDetailDto order)
        {
            if (shoppingCart.Any(s => s.productId == order.productId))
            {
                foreach (var item in shoppingCart)
                {
                    //aynı ürünü bulup, miktarını sipariş miktarı kadar artırıyoruz.
                    if (item.productId == order.productId)
                        item.productQuantity += order.productQuantity;
                }
            }
            else
            {
                //yeni ürün, ilk defa sepete atılacak.
                shoppingCart.Add(order);
            }
            return shoppingCart;
        }

        public List<ResultShoppingCartDetailDto> DeleteFromShoppingCart(List<ResultShoppingCartDetailDto> shoppingCart, int id)
        {
            shoppingCart.RemoveAll(s => s.productId == id);
            return shoppingCart;
        }

        public decimal TotalPrice(List<ResultShoppingCartDetailDto> shoppingCart)
        {
            var totalPrice = shoppingCart.Sum(s => s.productQuantity * s.productPrice);
            return totalPrice;
        }

        public int TotalQuantity(List<ResultShoppingCartDetailDto> shoppingCart)
        {
            var totalQuantity = shoppingCart.Sum(s => s.productQuantity);
            return totalQuantity;
        }

        public List<ResultShoppingCartDetailDto> UpdateQuantity(List<ResultShoppingCartDetailDto> shoppingCart, int productId, int newQuantity)
        {
            var item = shoppingCart.FirstOrDefault(x => x.productId == productId);
            if (item != null)
            {
                item.productQuantity = newQuantity;
            }
            return shoppingCart;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KitKap.Service.Dtos.ShoppingCartDtos
{
    public class AddCartItemDto
    {
        public long ProductId { get; set; }
        public int Quantity { get; set; }
    }
}

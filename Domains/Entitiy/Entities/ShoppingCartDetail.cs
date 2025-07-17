using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kitkap.Entity.Entities
{
    public class ShoppingCartDetail
    {
        public int productId { get; set; }
        public string productName { get; set; }
        public int productQuantity { get; set; }
        public decimal productPrice { get; set; }
    }
}

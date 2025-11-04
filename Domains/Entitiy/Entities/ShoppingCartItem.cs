using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kitkap.Entity.Entities
{
    public class ShoppingCartItem
    {
        public int Id { get; set; }
        public int ShoppingCartId { get; set; }  
        public long ProductId { get; set; }       

        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }   

        public ShoppingCart ShoppingCart { get; set; }
        public Product Product { get; set; }
    }
}

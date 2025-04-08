using Kitkap.Entity.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KitKap.DataAccess.Identity
{
    public class AppUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int? AddressId { get; set; }
        public Address Address { get; set; }
        public decimal Balance {  get; set; }
        public List<Product> Products { get; set; }


        public bool IsActived { get; set; } = true;
    }
}

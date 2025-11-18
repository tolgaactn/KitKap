using Kitkap.Entity.Entities;
using Kitkap.Entity.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.PowerBI.Api.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
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
        public List<LoginHistory> LoginHistories { get; set; }
        public bool IsActived { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}

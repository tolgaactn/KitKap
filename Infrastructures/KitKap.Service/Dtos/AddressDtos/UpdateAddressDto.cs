using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kitkap.Service.Dtos.AddressDtos
{
    public class UpdateAddressDto
    {
        public int AddressId { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public int PostCode { get; set; }
        public string UserId { get; set; }
        public string Description { get; set; }
    }
}

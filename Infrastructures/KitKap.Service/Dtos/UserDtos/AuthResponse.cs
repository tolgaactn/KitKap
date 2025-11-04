using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kitkap.Service.Dtos.AddressDtos
{
    public class AuthResponse
    {
        public bool IsSuccessful { get; set; }
        public string UserName { get; set; }
        public IEnumerable<string> Errors { get; set; } = new List<string>();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kitkap.Entity.Entities.Identity
{
    public class LoginHistory
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public DateTime LoginDate { get; set; }
        public string IpAddress { get; set; } = string.Empty;
        public bool IsSuccessful { get; set; }

    }
}

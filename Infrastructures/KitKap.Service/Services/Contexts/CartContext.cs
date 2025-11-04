using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KitKap.Service.Services.Contexts
{
    public class CartContext
    {
        public string? UserId { get; set; }
        public string? GuestId { get; set; }

        public bool IsGuest => string.IsNullOrEmpty(UserId) && !string.IsNullOrEmpty(GuestId);
        public bool IsAuthenticated => !string.IsNullOrEmpty(UserId);
    }
}

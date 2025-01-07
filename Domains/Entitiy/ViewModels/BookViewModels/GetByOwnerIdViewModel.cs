using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kitkap.Entity.ViewModels.BookViewModels
{
    public class GetByOwnerIdViewModel
    {
        public string Title { get; set; }
        public Decimal BookPoint { get; set; }
        public string OwnerId { get; set; }
        public int CategoryId { get; set; }
    }
}

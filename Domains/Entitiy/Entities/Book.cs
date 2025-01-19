using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kitkap.Entity.Entities
{
    public class Book : Product
    {
        public string Author { get; set; }
        public int ISBN { get; set; }
        public DateTime PublicationDate { get; set; }
        public string Language { get; set; }
        public string Condition { get; set; }

    }
}

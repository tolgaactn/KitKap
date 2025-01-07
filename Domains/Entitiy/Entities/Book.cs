using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kitkap.Entity.Entities
{
    public class Book
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public int ISBN { get; set; }
        public DateTime PublicationDate { get; set; }
        public string Language { get; set; }
        public Decimal BookPoint { get; set; }
        public int CategoryId {  get; set; }
        public bool IsAvailable { get; set; }
        public string Condition { get; set; }
        public string OwnerId { get; set; }


        public Category Category { get; set; }
    }
}

using Kitkap.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Kitkap.Entity.ViewModels.CategoryViewModels
{
    public class CreateCategoryViewModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? ParentCategoryId { get; set; }
        //public List<SelectListItem> Categories { get; set; }
    }
}

using Kitkap.Service.Dtos.AddressDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kitkap.Service.Dtos.AddressDtos
{
	public class UpdateBookDto : UpdateProductDto
	{
		public string Author { get; set; }
		public int ISBN { get; set; }
		public DateTime PublicationDate { get; set; }
		public string Language { get; set; }
		public string Condition { get; set; }
	}
}

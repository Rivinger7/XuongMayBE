using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XuongMay.ModelViews.ProductModelViews
{
	public class CreateProductModel
	{
		public string Name { get; set; }
		public string? Description { get; set; }
		public int CategoryId { get; set; }
	}
}

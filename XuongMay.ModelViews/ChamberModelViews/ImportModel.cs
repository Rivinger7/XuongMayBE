using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XuongMay.ModelViews.ChamberModelViews
{
	public class ImportModel
	{
		public required string Name { get; set; }
		public string? Description { get; set; }
		public List<int> ProductIds { get; set; } = new List<int>();
		public List<int> ChamberIds { get; set; } = new List<int>();
		public int Quantity { get; set; } 
		public int ItemPerBox { get; set; }
		
	}
}

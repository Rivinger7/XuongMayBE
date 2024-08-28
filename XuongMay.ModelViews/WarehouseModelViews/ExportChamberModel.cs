using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XuongMay.ModelViews.WarehouseModelViews
{
	public class ExportProductModel
	{
		public string InventoryName { get; set; }
		public int ProductId { get; set; }
		public int ChamberId { get; set; }
		public int ItemPerBox { get; set; }
		public int Quantity { get; set; }
	}
}

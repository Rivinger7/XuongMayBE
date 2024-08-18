using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XuongMay.ModelViews.OrderModelViews
{
	public class AllOrderModelView
	{
		public int Id { get; set; }

		public int ProductId { get; set; }

		public int Quantity { get; set; }

		public DateTime CreatedTime { get; set; }

		public DateTime StartTime { get; set; }

		public DateTime EndTime { get; set; }
	}
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XuongMay.ModelViews.TasksModelViews
{
	public class TasksCreateModel
	{
		[Required(ErrorMessage = "* Order Id can not be empty")]
		public int OrderId { get; set; }

		[Required(ErrorMessage = "* Assembly Id can not be empty")]
		public int AssemblyLineId { get; set; }

		[Required(ErrorMessage = "* Title can not be empty")]
		public string Title { get; set; }
		public string? Description { get; set; }

		[Required(ErrorMessage = "* Please enter quantity !")]
		public int Quantity { get; set; }

		[Required(ErrorMessage = "* Please enter start time !")]
		public DateTime StartTime { get; set; }

		[Required(ErrorMessage = "* Please enter end time !")]
		public DateTime EndTime { get; set; }
	}
}

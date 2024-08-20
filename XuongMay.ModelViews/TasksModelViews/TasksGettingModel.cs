using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XuongMay.ModelViews.TasksModelViews
{
	public class TasksGettingModel
	{
		public int Id { get; set; }
		public int OrderId { get; set; }
		public int AssemblyLineId { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public int Quantity { get; set; }
		public string StartTime { get; set; }
		public string EndTime { get; set; }
		public string? LastUpdatedTime { get; set; }
	}
}

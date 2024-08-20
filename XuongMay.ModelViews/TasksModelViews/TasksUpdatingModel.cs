using System.ComponentModel.DataAnnotations;
//using XuongMay.Core.Utils;

namespace XuongMay.ModelViews.TasksModelViews
{
	public class TasksUpdatingModel
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
		[RegularExpression(@"^(?:[01]\d|2[0-3]):[0-5]\d \d{2}/\d{2}/\d{4}$", ErrorMessage = "* Start time must be in the format HH:mm dd/MM/yyyy!")]
		public required string StartTime { get; set; }

		[Required(ErrorMessage = "* Please enter end time !")]
		[RegularExpression(@"^(?:[01]\d|2[0-3]):[0-5]\d \d{2}/\d{2}/\d{4}$", ErrorMessage = "* End time must be in the format HH:mm dd/MM/yyyy!")]
		public required string EndTime { get; set; }
	}
}

using System.ComponentModel.DataAnnotations;

namespace XuongMay.ModelViews.OrderModelViews
{
	public class UpdateOrderModelView
	{
		public int Quantity { get; set; }

		public int ProductId { get; set; }

		[RegularExpression(@"^(?:[01]\d|2[0-3]):[0-5]\d \d{2}/\d{2}/\d{4}$", ErrorMessage = "* Start time must be in the format HH:mm dd/MM/yyyy!")]
		public string StartTime { get; set; }

		[RegularExpression(@"^(?:[01]\d|2[0-3]):[0-5]\d \d{2}/\d{2}/\d{4}$", ErrorMessage = "* End time must be in the format HH:mm dd/MM/yyyy!")]
		public string EndTime { get; set; }
	}
}

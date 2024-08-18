namespace XuongMay.ModelViews.OrderModelViews
{
	public class AllOrderModelView
	{
		public int Id { get; set; }

		public string ProductName { get; set; } = string.Empty;

		public int Quantity { get; set; }

		public DateTime CreatedTime { get; set; }

		public DateTime StartTime { get; set; }

		public DateTime EndTime { get; set; }
	}
}

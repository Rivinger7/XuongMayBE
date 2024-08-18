namespace XuongMay.ModelViews.OrderModelViews
{
	public class AllOrderModelView
	{
		public int Id { get; set; }

		public string ProductName { get; set; } = string.Empty;

		public int Quantity { get; set; }

		public string CreatedTime { get; set; }

		public string StartTime { get; set; }

		public string EndTime { get; set; }
	}
}

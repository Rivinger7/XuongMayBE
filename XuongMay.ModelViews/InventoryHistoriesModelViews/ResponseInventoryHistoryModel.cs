namespace XuongMay.ModelViews.InventoryHistoriesModelViews
{
	public class ResponseInventoryHistoryModel
	{
		public int Id { get; set; }

		public string Name { get; set; } = string.Empty;

		public string ProducName {  get; set; } = string.Empty;

		public string? Description { get; set; }

		public int Quantity { get; set; }

		public int ItemPerBox { get; set; }

		public string CreatedTime { get; set; }

	}
}

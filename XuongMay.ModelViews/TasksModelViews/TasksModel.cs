namespace XuongMay.ModelViews.TasksModelViews
{
    public class TasksModel
	{
		public int Id { get; set; }
		public int OrderId { get; set; }
		public int AssemblyLineId { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public int Quantity { get; set; }
		public DateTime StartTime { get; set; }
		public DateTime EndTime { get; set; }
		public DateTime? LastUpdatedTime { get; set; }
	}
}

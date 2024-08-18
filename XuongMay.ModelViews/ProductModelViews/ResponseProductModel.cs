
namespace XuongMay.ModelViews.ProductModelViews
{
	public class ResponseProductModel
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string? Description { get; set; }
		public string Category { get; set; }
		public string CreatedTime { get; set; }
		public string? LastUpdateTime { get; set; }
		public string? DeletedTime { get; set; }
	}
}

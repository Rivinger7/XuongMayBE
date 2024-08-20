using System.Text.Json.Serialization;

namespace GarmentFactory.Contract.Repositories.Entity
{
	public class Tasks
	{
		public int Id { get; set; }
		public int OrderId { get; set; }
		public int AssemblyLineId { get; set; }
		public string Title { get; set; }
		public string? Description { get; set; }
		public int Quantity { get; set; }
		public DateTime StartTime { get; set; }
		public DateTime EndTime { get; set; }
		public DateTime? LastUpdatedTime { get; set; }
		public DateTime? DeletedTime { get; set; }
		public bool IsDeleted { get; set; } = false;

		#region entity Mapping
		[JsonIgnore]
		public virtual AssemblyLine AssemblyLine { get; set; }
		[JsonIgnore]
		public virtual Order Order { get; set; }
		#endregion
	}
}

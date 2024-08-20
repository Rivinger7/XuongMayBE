using System.Text.Json.Serialization;

namespace GarmentFactory.Contract.Repositories.Entity
{
	public class Order
	{
		public int Id { get; set; }
		public int ProductId { get; set; }
		public int Quantity { get; set; }
		public DateTime CreatedTime { get; set; }
		public DateTime StartTime { get; set; }
		public DateTime EndTime { get; set; }
		public DateTime? LastUpdatedTime { get; set; }
		public DateTime? DeletedTime { get; set; }
		public bool IsDeleted { get; set; } = false;

		#region entity Mapping
		public virtual ICollection<Tasks>? Tasks { get; set; }
		[JsonIgnore]
		public virtual Product Product { get; set; }
		#endregion
	}
}

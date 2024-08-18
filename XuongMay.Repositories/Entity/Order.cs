namespace GarmentFactory.Repository.Entities
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
		public bool IsDeleted { get; set; }

		#region entity Mapping
		public virtual ICollection<Tasks>? Tasks { get; set; }
		public virtual Product Product { get; set; }
		#endregion
	}
}

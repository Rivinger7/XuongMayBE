namespace GarmentFactory.Contract.Repositories.Entity
{
	public class Category
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string? Description { get; set; }
		public DateTime CreatedTime { get; set; }
		public DateTime? LastUpdatedTime { get; set; }
		public DateTime? DeletedTime { get; set; }
		public bool IsDeleted { get; set; } = false;

		#region entity Mapping
		public virtual ICollection<Product> Products { get; set; }
		#endregion
	}

}

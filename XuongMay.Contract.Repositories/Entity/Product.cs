using System.ComponentModel.DataAnnotations;
using XuongMay.Contract.Repositories.Entity;

namespace GarmentFactory.Contract.Repositories.Entity
{
	public class Product
	{
		[Key]
		public int Id { get; set; }
		public int CategoryId { get; set; }
		public string Name { get; set; }
		public string? Description { get; set; }
		public DateTime CreatedTime { get; set; }
		public DateTime? LastUpdateTime { get; set;}
		public DateTime? DeletedTime { get; set; }
		public bool IsDeleted { get; set; } = false;

		#region entity Mapping
		public virtual ICollection<Order>? Orders { get; set; }
		public virtual Category Category { get; set; }
		public virtual ICollection<InventoryHistories>? InventoryHistories { get; set; }
		#endregion
	}
}

using GarmentFactory.Contract.Repositories.Entity;
using System.ComponentModel.DataAnnotations;

namespace XuongMay.Contract.Repositories.Entity
{
	public class InventoryHistories
    {
        [Key]
        public int Id { get; set; }
        public int ProductId { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public bool IsImport { get; set; }
        public int TotalQuantity { get; set; }
        public required string CreatedBy { get; set; }
        public string? LastUpdatedBy { get; set;}
        public string? DeletedBy { get; set; }
        public required DateTime CreatedTime { get; set;}
        public DateTime? LastUpdatedTime { get; set; }
        public DateTime? DeletedTime { get; set; }
        public int ItemPerBox { get; set; }

        #region Entity Mapping
        public virtual ICollection<InventoryChamberMappers>? InventoryChamberMappers { get; set; }
        public virtual Product Product { get; set; } = null!;
        #endregion
    }
}

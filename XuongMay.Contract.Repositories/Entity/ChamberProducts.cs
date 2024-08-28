using System.ComponentModel.DataAnnotations;

namespace XuongMay.Contract.Repositories.Entity
{
	public class ChamberProducts
    {
        [Key]
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public int Capacity { get; set; }
        public int Quantity { get; set; }
        public required string CreatedBy { get; set; }
        public string? LastUpdatedBy { get; set; }
        public string? DeletedBy { get; set; }
        public required DateTime CreatedTime { get; set; }
        public DateTime? LastUpdatedTime { get; set; }
        public DateTime? DeletedTime { get; set; }
        public string? LastInventoryHistory { get; set; }

        #region Entity Mapping
        public virtual ICollection<InventoryChamberMappers>? InventoryChamberMappers { get; set; }
        #endregion
    }
}

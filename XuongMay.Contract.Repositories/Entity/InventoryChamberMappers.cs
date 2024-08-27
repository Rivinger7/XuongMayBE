using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XuongMay.Contract.Repositories.Entity
{
    public class InventoryChamberMappers
    {
        [Key]
        public int Id { get; set; }
        public int InventoryId { get; set; }
        public int ChamberId { get; set; }
        public int Quantity { get; set; }
        public required string CreatedBy { get; set; }
        public string? LastUpdatedBy { get; set; }
        public string? DeletedBy { get; set; }
        public required DateTime CreatedTime { get; set; }
        public DateTime? LastUpdatedTime { get; set; }
        public DateTime? DeletedTime { get; set; }

        #region Entity Mapping
        public virtual InventoryHistories InventoryHistories { get; set; } = null!;
        public virtual ChamberProducts ChamberProducts { get; set; } = null!;
        #endregion
    }
}

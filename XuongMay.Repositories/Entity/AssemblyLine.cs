using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarmentFactory.Repository.Entities
{
	public class AssemblyLine
	{
		public int Id { get; set; }
		public int ManagerId { get; set; }
		public string Name { get; set; } = null!;
		public string? Description { get; set; }
		public int NumberOfStaffs { get; set; }
		public string CreatedBy { get; set; } = null!;
		public DateTime CreatedTime { get; set; }
		public DateTime? LastUpdatedTime { get; set; }
		public DateTime? DeletedTime { get; set; }
		public bool IsDeleted { get; set; } = false;

		#region entity Mapping
		public virtual User User { get; set; }
		public virtual ICollection<Tasks> Tasks { get; set; }
		#endregion
	}
}

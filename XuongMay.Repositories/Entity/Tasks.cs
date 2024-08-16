using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarmentFactory.Repository.Entities
{
	public class Tasks
	{
		public int Id { get; set; }
		public int OrderId { get; set; }
		public int AssemblyLineId { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public int Quantity { get; set; }
		public DateTime StartTime { get; set; }
		public DateTime EndTime { get; set; }
		public DateTime LastUpdatedTime { get; set; }
		public DateTime DeletedTime { get; set; }
		public bool IsDeleted { get; set; }
		public string Status { get; set; }

		#region entity Mapping
		public virtual AssemblyLine AssemblyLine { get; set; }
		public virtual Order Order { get; set; }
		#endregion
	}
}

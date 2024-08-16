﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarmentFactory.Repository.Entities
{
	public class Product
	{
		[Key]
		public int Id { get; set; }
		public int CategoryId { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public DateTime CreatedTime { get; set; }
		public DateTime LastUpdateTime { get; set;}
		public DateTime DeletedTime { get; set; }
		public bool IsDeleted { get; set; }

		#region entity Mapping
		public virtual ICollection<Order>? Orders { get; set; }
		public virtual Category Category { get; set; }
		#endregion
	}
}

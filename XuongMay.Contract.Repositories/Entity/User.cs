﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarmentFactory.Repository.Entities
{
	public class User
	{
		[Key]
		public int Id { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }
		public string Role { get; set; }
		public string FullName { get; set; }
		public DateTime CreatedTime { get; set; }
		public DateTime LastUpdatedTime { get; set; }
		public DateTime DeletedTime { get; set; }
		public bool IsDeleted { get; set; }

		#region entity Mapping
		public virtual AssemblyLine AssemblyLine { get; set; }
		#endregion
	}
}

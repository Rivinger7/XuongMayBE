using System.ComponentModel.DataAnnotations;

namespace GarmentFactory.Repository.Entities
{
    public class User
	{
		[Key]
		public int Id { get; set; }
		public string Username { get; set; } = null!;
		public string Password { get; set; } = null!;
		public string Role { get; set; } = null!;
		public string FullName { get; set; } = null!;
		public DateTime CreatedTime { get; set; }
		public DateTime? LastUpdatedTime { get; set; }
		public DateTime? DeletedTime { get; set; }
		public bool IsDeleted { get; set; } = false;
		public string? RefreshToken { get; set; }
		public DateTime? RefreshTokenExpiryTime { get; set; }

		#region entity Mapping
		public virtual AssemblyLine AssemblyLine { get; set; }
		#endregion
	}
}

using GarmentFactory.Contract.Repositories.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarmentFactory.Repository.Context
{
	public partial class GarmentFactoryDBContext : DbContext
	{
		// constructor default
		public GarmentFactoryDBContext() { }

		// constructor with options if any
		public GarmentFactoryDBContext(DbContextOptions<GarmentFactoryDBContext> options) : base(options) { }

		#region DbSet
		public virtual DbSet<User> Users { get; set; }
		public virtual DbSet<AssemblyLine> AssemblyLines { get; set; }
		public virtual DbSet<Category> Categories { get; set; }
		public virtual DbSet<Order> Orders { get; set; }
		public virtual DbSet<Product> Products { get; set; }
		public virtual DbSet<Tasks> Tasks { get; set; }
		#endregion

		// config connection string default if not set
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			if (!optionsBuilder.IsConfigured)
			{
				optionsBuilder.UseSqlServer("Server=.;Database=GarmentFactoryDB;Trusted_Connection=True;", b => b.MigrationsAssembly("GarmentFactory.Repository"));
			}
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<User>(entity =>
			{
				entity.HasKey(e => e.Id); // set Primary Key

				entity.Property(e => e.Id).ValueGeneratedOnAdd(); // set Id Auto Increment

			});

			modelBuilder.Entity<AssemblyLine>(entity =>
			{
				entity.HasKey(e => e.Id); // set Primary Key

				entity.Property(e => e.Id).ValueGeneratedOnAdd(); // set Id Auto Increment

				entity.Property(e => e.CreatedBy).HasMaxLength(50); // set max length

				entity.Property(e => e.CreatedTime).HasColumnType("datetime"); // set data type

				entity.Property(e => e.LastUpdatedTime).HasColumnType("datetime");

				entity.Property(e => e.DeletedTime).HasColumnType("datetime");

				entity.Property(e => e.Description).HasMaxLength(1000);

				entity.Property(e => e.Name).HasMaxLength(50);

				entity.HasOne<User>(u => u.User).WithOne(al => al.AssemblyLine).HasForeignKey<AssemblyLine>(al => al.ManagerId).HasConstraintName("FK_UserId_AssemblyLineId");
			});

			modelBuilder.Entity<Category>(entity =>
			{
				entity.HasKey(e => e.Id); // set Primary Key

				entity.Property(e => e.Id).ValueGeneratedOnAdd(); // set Id Auto Increment

				entity.Property(e => e.CreatedTime).HasColumnType("datetime");

				entity.Property(e => e.LastUpdatedTime).HasColumnType("datetime");

				entity.Property(e => e.DeletedTime).HasColumnType("datetime");

				entity.Property(e => e.Description).HasMaxLength(500);

				entity.Property(e => e.Name).HasMaxLength(50);
			});

			modelBuilder.Entity<Order>(entity =>
			{
				entity.HasKey(e => e.Id); // set Primary Key

				entity.Property(e => e.Id).ValueGeneratedOnAdd(); // set Id Auto Increment

				entity.Property(e => e.CreatedTime).HasColumnType("datetime");

				entity.Property(e => e.LastUpdatedTime).HasColumnType("datetime");

				entity.Property(e => e.DeletedTime).HasColumnType("datetime");
			});

			modelBuilder.Entity<Tasks>(entity =>
			{
				entity.HasKey(e => e.Id); // set Primary Key

				entity.Property(e => e.Id).ValueGeneratedOnAdd(); // set Id Auto Increment

				// Many to Many Relationship
				entity.HasOne<AssemblyLine>(a => a.AssemblyLine).WithMany(t => t.Tasks).HasForeignKey(t => t.AssemblyLineId).HasConstraintName("FK_AssemblyLine_Task");

				entity.HasOne<Order>(a => a.Order).WithMany(t => t.Tasks).HasForeignKey(t => t.OrderId).HasConstraintName("FK_Order_Task");

				entity.Property(e => e.StartTime).HasColumnType("datetime");

				entity.Property(e => e.EndTime).HasColumnType("datetime");

				entity.Property(e => e.LastUpdatedTime).HasColumnType("datetime");

				entity.Property(e => e.Title).HasMaxLength(1000);

				entity.Property(e => e.Description).HasMaxLength(1000);

				entity.Property(e => e.DeletedTime).HasColumnType("datetime");
			});

			modelBuilder.Entity<Product>(entity =>
			{
				entity.HasKey(e => e.Id); // set Primary Key

				entity.Property(e => e.Id).ValueGeneratedOnAdd(); // set Id Auto Increment

				entity.Property(e => e.CreatedTime).HasColumnType("datetime");

				entity.Property(e => e.LastUpdateTime).HasColumnType("datetime");

				entity.Property(e => e.DeletedTime).HasColumnType("datetime");

				entity.Property(e => e.Description).HasMaxLength(1000);

				entity.Property(e => e.Name).HasMaxLength(50);
			});

			OnModelCreatingPartial(modelBuilder);
		}

		partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
	}
}

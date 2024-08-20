﻿// <auto-generated />
using System;
using GarmentFactory.Repository.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace GarmentFactory.Repository.Migrations
{
    [DbContext(typeof(GarmentFactoryDBContext))]
    [Migration("20240819034436_MigrationV9Hoang")]
    partial class MigrationV9Hoang
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Proxies:ChangeTracking", false)
                .HasAnnotation("Proxies:CheckEquality", false)
                .HasAnnotation("Proxies:LazyLoading", true)
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("GarmentFactory.Repository.Entities.AssemblyLine", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<DateTime>("CreatedTime")
                        .HasColumnType("datetime");

                    b.Property<DateTime?>("DeletedTime")
                        .HasColumnType("datetime");

                    b.Property<string>("Description")
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("LastUpdatedTime")
                        .HasColumnType("datetime");

                    b.Property<int>("ManagerId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int>("NumberOfStaffs")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ManagerId")
                        .IsUnique();

                    b.ToTable("AssemblyLines");
                });

            modelBuilder.Entity("GarmentFactory.Repository.Entities.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedTime")
                        .HasColumnType("datetime");

                    b.Property<DateTime?>("DeletedTime")
                        .HasColumnType("datetime");

                    b.Property<string>("Description")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("LastUpdatedTime")
                        .HasColumnType("datetime");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("GarmentFactory.Repository.Entities.Order", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedTime")
                        .HasColumnType("datetime");

                    b.Property<DateTime?>("DeletedTime")
                        .HasColumnType("datetime");

                    b.Property<DateTime>("EndTime")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("LastUpdatedTime")
                        .HasColumnType("datetime");

                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<DateTime>("StartTime")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("ProductId");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("GarmentFactory.Repository.Entities.Product", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("CategoryId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedTime")
                        .HasColumnType("datetime");

                    b.Property<DateTime?>("DeletedTime")
                        .HasColumnType("datetime");

                    b.Property<string>("Description")
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("LastUpdateTime")
                        .HasColumnType("datetime");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("GarmentFactory.Repository.Entities.Tasks", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("AssemblyLineId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("DeletedTime")
                        .HasColumnType("datetime");

                    b.Property<string>("Description")
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)");

                    b.Property<DateTime>("EndTime")
                        .HasColumnType("datetime");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("LastUpdatedTime")
                        .HasColumnType("datetime");

                    b.Property<int>("OrderId")
                        .HasColumnType("int");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<DateTime>("StartTime")
                        .HasColumnType("datetime");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)");

                    b.HasKey("Id");

                    b.HasIndex("AssemblyLineId");

                    b.HasIndex("OrderId");

                    b.ToTable("Tasks");
                });

            modelBuilder.Entity("GarmentFactory.Repository.Entities.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedTime")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DeletedTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("LastUpdatedTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RefreshToken")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("RefreshTokenExpiryTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("GarmentFactory.Repository.Entities.AssemblyLine", b =>
                {
                    b.HasOne("GarmentFactory.Repository.Entities.User", "User")
                        .WithOne("AssemblyLine")
                        .HasForeignKey("GarmentFactory.Repository.Entities.AssemblyLine", "ManagerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_UserId_AssemblyLineId");

                    b.Navigation("User");
                });

            modelBuilder.Entity("GarmentFactory.Repository.Entities.Order", b =>
                {
                    b.HasOne("GarmentFactory.Repository.Entities.Product", "Product")
                        .WithMany("Orders")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Product");
                });

            modelBuilder.Entity("GarmentFactory.Repository.Entities.Product", b =>
                {
                    b.HasOne("GarmentFactory.Repository.Entities.Category", "Category")
                        .WithMany("Products")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Category");
                });

            modelBuilder.Entity("GarmentFactory.Repository.Entities.Tasks", b =>
                {
                    b.HasOne("GarmentFactory.Repository.Entities.AssemblyLine", "AssemblyLine")
                        .WithMany("Tasks")
                        .HasForeignKey("AssemblyLineId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_AssemblyLine_Task");

                    b.HasOne("GarmentFactory.Repository.Entities.Order", "Order")
                        .WithMany("Tasks")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_Order_Task");

                    b.Navigation("AssemblyLine");

                    b.Navigation("Order");
                });

            modelBuilder.Entity("GarmentFactory.Repository.Entities.AssemblyLine", b =>
                {
                    b.Navigation("Tasks");
                });

            modelBuilder.Entity("GarmentFactory.Repository.Entities.Category", b =>
                {
                    b.Navigation("Products");
                });

            modelBuilder.Entity("GarmentFactory.Repository.Entities.Order", b =>
                {
                    b.Navigation("Tasks");
                });

            modelBuilder.Entity("GarmentFactory.Repository.Entities.Product", b =>
                {
                    b.Navigation("Orders");
                });

            modelBuilder.Entity("GarmentFactory.Repository.Entities.User", b =>
                {
                    b.Navigation("AssemblyLine")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}

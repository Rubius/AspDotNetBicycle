﻿// <auto-generated />
using System;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Infrastructure.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20220215154949_AddUserRefreshTokenTable")]
    partial class AddUserRefreshTokenTable
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("Domain.Entities.Bicycle", b =>
                {
                    b.Property<decimal>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("decimal(20,0)");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<decimal>("Id"), 1L, 1);

                    b.Property<bool>("IsWrittenOff")
                        .HasColumnType("bit");

                    b.Property<DateTime>("ManufactureDate")
                        .HasColumnType("datetime2");

                    b.Property<decimal>("ModelId")
                        .HasColumnType("decimal(20,0)");

                    b.HasKey("Id");

                    b.HasIndex("ModelId");

                    b.ToTable("Bicycles");
                });

            modelBuilder.Entity("Domain.Entities.BicycleModel", b =>
                {
                    b.Property<decimal>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("decimal(20,0)");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<decimal>("Id"), 1L, 1);

                    b.Property<int>("LifeTimeYears")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("BicycleModels");
                });

            modelBuilder.Entity("Domain.Entities.UserRefreshToken", b =>
                {
                    b.Property<decimal>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("decimal(20,0)");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<decimal>("Id"), 1L, 1);

                    b.Property<DateTime>("ExpiredAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("RefreshToken")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RequestedIpAddress")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.ToTable("UserRefreshTokens");
                });

            modelBuilder.Entity("Domain.Entities.Bicycle", b =>
                {
                    b.HasOne("Domain.Entities.BicycleModel", "Model")
                        .WithMany("Bicycles")
                        .HasForeignKey("ModelId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.OwnsOne("Domain.Entities.ValueObjects.Address", "RentalPointAddress", b1 =>
                        {
                            b1.Property<decimal>("BicycleId")
                                .HasColumnType("decimal(20,0)");

                            b1.Property<string>("City")
                                .HasColumnType("nvarchar(max)")
                                .HasColumnName("RentalPointCity");

                            b1.Property<string>("Country")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)")
                                .HasColumnName("RentalPointCountry");

                            b1.Property<string>("Region")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)")
                                .HasColumnName("RentalPointRegion");

                            b1.Property<string>("Street")
                                .HasColumnType("nvarchar(max)")
                                .HasColumnName("RentalPointStreet");

                            b1.HasKey("BicycleId");

                            b1.ToTable("Bicycles");

                            b1.WithOwner()
                                .HasForeignKey("BicycleId");
                        });

                    b.Navigation("Model");

                    b.Navigation("RentalPointAddress")
                        .IsRequired();
                });

            modelBuilder.Entity("Domain.Entities.BicycleModel", b =>
                {
                    b.OwnsOne("Domain.Entities.ValueObjects.Address", "ManufacturerAddress", b1 =>
                        {
                            b1.Property<decimal>("BicycleModelId")
                                .HasColumnType("decimal(20,0)");

                            b1.Property<string>("City")
                                .HasColumnType("nvarchar(max)")
                                .HasColumnName("ManufacturerCity");

                            b1.Property<string>("Country")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)")
                                .HasColumnName("ManufacturerCountry");

                            b1.Property<string>("Region")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)")
                                .HasColumnName("ManufacturerRegion");

                            b1.Property<string>("Street")
                                .HasColumnType("nvarchar(max)")
                                .HasColumnName("ManufacturerStreet");

                            b1.HasKey("BicycleModelId");

                            b1.ToTable("BicycleModels");

                            b1.WithOwner()
                                .HasForeignKey("BicycleModelId");
                        });

                    b.Navigation("ManufacturerAddress")
                        .IsRequired();
                });

            modelBuilder.Entity("Domain.Entities.BicycleModel", b =>
                {
                    b.Navigation("Bicycles");
                });
#pragma warning restore 612, 618
        }
    }
}

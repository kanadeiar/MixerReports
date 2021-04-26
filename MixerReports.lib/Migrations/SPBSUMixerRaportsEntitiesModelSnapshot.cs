﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MixerReports.lib.Data.Base;

namespace MixerReports.lib.Migrations
{
    [DbContext(typeof(SPBSUMixerRaportsEntities))]
    partial class SPBSUMixerRaportsEntitiesModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.5")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("MixerReports.lib.Models.Mix", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<float>("ActAluminium1")
                        .HasColumnType("real");

                    b.Property<float>("ActAluminium2")
                        .HasColumnType("real");

                    b.Property<float>("ActCement1")
                        .HasColumnType("real");

                    b.Property<float>("ActCement2")
                        .HasColumnType("real");

                    b.Property<float>("ActColdWater")
                        .HasColumnType("real");

                    b.Property<float>("ActHotWater")
                        .HasColumnType("real");

                    b.Property<float>("ActMixture1")
                        .HasColumnType("real");

                    b.Property<float>("ActMixture2")
                        .HasColumnType("real");

                    b.Property<float>("ActRevertMud")
                        .HasColumnType("real");

                    b.Property<float>("ActSandMud")
                        .HasColumnType("real");

                    b.Property<bool>("Boiled")
                        .HasColumnType("bit");

                    b.Property<string>("Comment")
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.Property<DateTime>("DateTime")
                        .HasColumnType("datetime2");

                    b.Property<float>("DensityRevertMud")
                        .HasColumnType("real");

                    b.Property<float>("DensitySandMud")
                        .HasColumnType("real");

                    b.Property<int>("FormNumber")
                        .HasColumnType("int");

                    b.Property<float>("MixerTemperature")
                        .HasColumnType("real");

                    b.Property<bool>("Normal")
                        .HasColumnType("bit");

                    b.Property<int>("Number")
                        .HasColumnType("int");

                    b.Property<bool>("Other")
                        .HasColumnType("bit");

                    b.Property<bool>("Overground")
                        .HasColumnType("bit");

                    b.Property<int>("RecipeNumber")
                        .HasColumnType("int");

                    b.Property<float>("SandInMud")
                        .HasColumnType("real");

                    b.Property<float>("SetAluminium1")
                        .HasColumnType("real");

                    b.Property<float>("SetAluminium2")
                        .HasColumnType("real");

                    b.Property<float>("SetCement1")
                        .HasColumnType("real");

                    b.Property<float>("SetCement2")
                        .HasColumnType("real");

                    b.Property<float>("SetColdWater")
                        .HasColumnType("real");

                    b.Property<float>("SetHotWater")
                        .HasColumnType("real");

                    b.Property<float>("SetMixture1")
                        .HasColumnType("real");

                    b.Property<float>("SetMixture2")
                        .HasColumnType("real");

                    b.Property<float>("SetRevertMud")
                        .HasColumnType("real");

                    b.Property<float>("SetSandMud")
                        .HasColumnType("real");

                    b.Property<byte[]>("Timestamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("rowversion");

                    b.Property<bool>("Undersized")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.ToTable("Mixes");
                });
#pragma warning restore 612, 618
        }
    }
}

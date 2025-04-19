﻿// <auto-generated />
using DAL2;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace DAL2.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20250419105534_AddDescriptionToBook")]
    partial class AddDescriptionToBook
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.4");

            modelBuilder.Entity("DAL2.Entities.Content", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("ContentType")
                        .IsRequired()
                        .HasMaxLength(8)
                        .HasColumnType("TEXT");

                    b.Property<string>("Format")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Contents");

                    b.HasDiscriminator<string>("ContentType").HasValue("Content");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("DAL2.Entities.ContentLocation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("ContentId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("StorageId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("ContentId")
                        .IsUnique();

                    b.HasIndex("StorageId");

                    b.ToTable("ContentLocations");
                });

            modelBuilder.Entity("DAL2.Entities.Storage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("LocationName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Storages");
                });

            modelBuilder.Entity("DAL2.Entities.Audio", b =>
                {
                    b.HasBaseType("DAL2.Entities.Content");

                    b.Property<string>("Artist")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<double>("Duration")
                        .HasColumnType("REAL");

                    b.HasDiscriminator().HasValue("Audio");
                });

            modelBuilder.Entity("DAL2.Entities.Book", b =>
                {
                    b.HasBaseType("DAL2.Entities.Content");

                    b.Property<string>("Author")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("PageCount")
                        .HasColumnType("INTEGER");

                    b.HasDiscriminator().HasValue("Book");
                });

            modelBuilder.Entity("DAL2.Entities.Document", b =>
                {
                    b.HasBaseType("DAL2.Entities.Content");

                    b.Property<string>("Author")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.ToTable("Contents", t =>
                        {
                            t.Property("Author")
                                .HasColumnName("Document_Author");
                        });

                    b.HasDiscriminator().HasValue("Document");
                });

            modelBuilder.Entity("DAL2.Entities.Video", b =>
                {
                    b.HasBaseType("DAL2.Entities.Content");

                    b.Property<string>("Director")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("ResolutionHeight")
                        .HasColumnType("INTEGER");

                    b.HasDiscriminator().HasValue("Video");
                });

            modelBuilder.Entity("DAL2.Entities.ContentLocation", b =>
                {
                    b.HasOne("DAL2.Entities.Content", "Content")
                        .WithOne("Location")
                        .HasForeignKey("DAL2.Entities.ContentLocation", "ContentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DAL2.Entities.Storage", "Storage")
                        .WithMany("ContentLocations")
                        .HasForeignKey("StorageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Content");

                    b.Navigation("Storage");
                });

            modelBuilder.Entity("DAL2.Entities.Content", b =>
                {
                    b.Navigation("Location");
                });

            modelBuilder.Entity("DAL2.Entities.Storage", b =>
                {
                    b.Navigation("ContentLocations");
                });
#pragma warning restore 612, 618
        }
    }
}

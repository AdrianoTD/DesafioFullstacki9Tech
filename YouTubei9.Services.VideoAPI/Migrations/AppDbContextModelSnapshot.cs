﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using YouTubei9.Services.VideoAPI.Data;

#nullable disable

namespace YouTubei9.Services.VideoAPI.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.12")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("YouTubei9.Services.VideoAPI.Models.VideoSearchComponents.ThumbnailItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("Height")
                        .HasColumnType("int");

                    b.Property<int>("ThumbType")
                        .HasColumnType("int");

                    b.Property<string>("Url")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("Width")
                        .HasColumnType("int");

                    b.Property<int?>("YTBVideoSearchId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("YTBVideoSearchId");

                    b.ToTable("YTBVideoSearchesThumbs");
                });

            modelBuilder.Entity("YouTubei9.Services.VideoAPI.Models.YTBVideoSearch", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ChannelDescription")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ChannelTitle")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("Date")
                        .HasColumnType("datetime2");

                    b.Property<string>("Duration")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("VideoId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("VideoTitle")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("YTBVideoSearches");
                });

            modelBuilder.Entity("YouTubei9.Services.VideoAPI.Models.VideoSearchComponents.ThumbnailItem", b =>
                {
                    b.HasOne("YouTubei9.Services.VideoAPI.Models.YTBVideoSearch", null)
                        .WithMany("Thumbnails")
                        .HasForeignKey("YTBVideoSearchId");
                });

            modelBuilder.Entity("YouTubei9.Services.VideoAPI.Models.YTBVideoSearch", b =>
                {
                    b.Navigation("Thumbnails");
                });
#pragma warning restore 612, 618
        }
    }
}

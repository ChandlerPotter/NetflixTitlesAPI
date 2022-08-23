﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NetflixTitles.API.DbContexts;

#nullable disable

namespace NetflixTitles.API.Migrations
{
    [DbContext(typeof(netflix_appContext))]
    [Migration("20220719004602_NetflixTitlesDBAddUserRatingAndWatchedToTitleLists")]
    partial class NetflixTitlesDBAddUserRatingAndWatchedToTitleLists
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseCollation("utf8mb4_0900_ai_ci")
                .HasAnnotation("ProductVersion", "6.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.HasCharSet(modelBuilder, "utf8mb4");

            modelBuilder.Entity("NetflixTitles.API.Entities.List", b =>
                {
                    b.Property<int>("ListId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("list_id");

                    b.Property<string>("ListName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)")
                        .HasColumnName("list_name");

                    b.Property<int?>("UserId")
                        .HasColumnType("int")
                        .HasColumnName("user_id");

                    b.HasKey("ListId");

                    b.HasIndex(new[] { "UserId" }, "par_ind");

                    b.ToTable("lists", (string)null);
                });

            modelBuilder.Entity("NetflixTitles.API.Entities.Title", b =>
                {
                    b.Property<int>("TitleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("title_id");

                    b.Property<string>("Cast")
                        .HasColumnType("text")
                        .HasColumnName("cast");

                    b.Property<string>("Country")
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)")
                        .HasColumnName("country");

                    b.Property<DateOnly?>("DateAdded")
                        .HasColumnType("date")
                        .HasColumnName("date_added");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("description");

                    b.Property<string>("Director")
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)")
                        .HasColumnName("director");

                    b.Property<string>("Duration")
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)")
                        .HasColumnName("duration");

                    b.Property<string>("ListedIn")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)")
                        .HasColumnName("listed_in");

                    b.Property<string>("Rating")
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)")
                        .HasColumnName("rating");

                    b.Property<short?>("ReleaseYear")
                        .HasColumnType("year")
                        .HasColumnName("release_year");

                    b.Property<string>("TitleName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)")
                        .HasColumnName("title_name");

                    b.Property<string>("Type")
                        .HasColumnType("enum('Movie','TV Show')")
                        .HasColumnName("type");

                    b.Property<int?>("UserRating")
                        .HasColumnType("int")
                        .HasColumnName("user_rating");

                    b.Property<bool?>("Watched")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint(1)")
                        .HasColumnName("watched")
                        .HasDefaultValueSql("'0'");

                    b.HasKey("TitleId");

                    b.ToTable("titles", (string)null);
                });

            modelBuilder.Entity("NetflixTitles.API.Entities.TitleList", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    b.Property<int?>("ListId")
                        .HasColumnType("int")
                        .HasColumnName("list_id");

                    b.Property<int?>("TitleId")
                        .HasColumnType("int")
                        .HasColumnName("title_id");

                    b.Property<int?>("UserRating")
                        .HasColumnType("int");

                    b.Property<bool?>("Watched")
                        .HasColumnType("tinyint(1)");

                    b.HasKey("Id");

                    b.HasIndex(new[] { "ListId" }, "list_id");

                    b.HasIndex(new[] { "TitleId" }, "title_id");

                    b.ToTable("title_lists", (string)null);
                });

            modelBuilder.Entity("NetflixTitles.API.Entities.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("user_id");

                    b.Property<DateTime?>("DateCreated")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasColumnName("date_created")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)")
                        .HasColumnName("email");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)")
                        .HasColumnName("password");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)")
                        .HasColumnName("user_name");

                    b.Property<string>("UserType")
                        .HasColumnType("enum('standard','admin')")
                        .HasColumnName("user_type");

                    b.HasKey("UserId");

                    b.ToTable("users", (string)null);
                });

            modelBuilder.Entity("NetflixTitles.API.Entities.List", b =>
                {
                    b.HasOne("NetflixTitles.API.Entities.User", "User")
                        .WithMany("Lists")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .HasConstraintName("fk_user");

                    b.Navigation("User");
                });

            modelBuilder.Entity("NetflixTitles.API.Entities.TitleList", b =>
                {
                    b.HasOne("NetflixTitles.API.Entities.List", "List")
                        .WithMany("TitleLists")
                        .HasForeignKey("ListId")
                        .HasConstraintName("title_lists_ibfk_1");

                    b.HasOne("NetflixTitles.API.Entities.Title", "Title")
                        .WithMany("TitleLists")
                        .HasForeignKey("TitleId")
                        .HasConstraintName("title_lists_ibfk_2");

                    b.Navigation("List");

                    b.Navigation("Title");
                });

            modelBuilder.Entity("NetflixTitles.API.Entities.List", b =>
                {
                    b.Navigation("TitleLists");
                });

            modelBuilder.Entity("NetflixTitles.API.Entities.Title", b =>
                {
                    b.Navigation("TitleLists");
                });

            modelBuilder.Entity("NetflixTitles.API.Entities.User", b =>
                {
                    b.Navigation("Lists");
                });
#pragma warning restore 612, 618
        }
    }
}

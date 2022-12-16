using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using NetflixTitles.API.Entities;

namespace NetflixTitles.API.DbContexts
{
    public partial class netflix_appContext : DbContext
    {

        public netflix_appContext(DbContextOptions<netflix_appContext> options)
            : base(options)
        {
        }

        public virtual DbSet<List> Lists { get; set; } = null!;
        public virtual DbSet<Title> Titles { get; set; } = null!;
        public virtual DbSet<TitleList> TitleLists { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseCollation("utf8mb4_0900_ai_ci")
                .HasCharSet("utf8mb4");

            modelBuilder.Entity<List>(entity =>
            {
                entity.ToTable("lists");

                entity.HasIndex(e => e.UserId, "par_ind");

                entity.Property(e => e.ListId).HasColumnName("list_id");

                entity.Property(e => e.ListName)
                    .HasMaxLength(255)
                    .HasColumnName("list_name");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Lists)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_user");
            });

            modelBuilder.Entity<Title>(entity =>
            {
                entity.ToTable("titles");

                entity.Property(e => e.TitleId).HasColumnName("title_id");

                entity.Property(e => e.Cast)
                    .HasColumnType("text")
                    .HasColumnName("cast");

                entity.Property(e => e.Country)
                    .HasMaxLength(255)
                    .HasColumnName("country");

                entity.Property(e => e.DateAdded).HasColumnName("date_added");

                entity.Property(e => e.Description)
                    .HasColumnType("text")
                    .HasColumnName("description");

                entity.Property(e => e.Director)
                    .HasMaxLength(255)
                    .HasColumnName("director");

                entity.Property(e => e.Duration)
                    .HasMaxLength(255)
                    .HasColumnName("duration");

                entity.Property(e => e.ListedIn)
                    .HasMaxLength(255)
                    .HasColumnName("listed_in");

                entity.Property(e => e.Rating)
                    .HasMaxLength(255)
                    .HasColumnName("rating");

                entity.Property(e => e.ReleaseYear)
                    .HasColumnType("year")
                    .HasColumnName("release_year");

                entity.Property(e => e.TitleName)
                    .HasMaxLength(255)
                    .HasColumnName("title_name");

                entity.Property(e => e.Type)
                    .HasColumnType("enum('Movie','TV Show')")
                    .HasColumnName("type");

                entity.Property(e => e.UserRating).HasColumnName("user_rating");

                entity.Property(e => e.Watched)
                    .HasColumnName("watched")
                    .HasDefaultValueSql("'0'");
            });

            modelBuilder.Entity<TitleList>(entity =>
            {
                entity.ToTable("title_lists");

                entity.HasIndex(e => e.ListId, "list_id");

                entity.HasIndex(e => e.TitleId, "title_id");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ListId).HasColumnName("list_id");

                entity.Property(e => e.TitleId).HasColumnName("title_id");

                entity.Property(e => e.UserRating).HasColumnName("UserRating");

                entity.Property(e => e.Watched)
                .HasColumnName("Watched")
                .HasDefaultValueSql("'0'");


                entity.HasOne(d => d.List)
                    .WithMany(p => p.TitleLists)
                    .HasForeignKey(d => d.ListId)
                    .HasConstraintName("title_lists_ibfk_1");

                entity.HasOne(d => d.Title)
                    .WithMany(p => p.TitleLists)
                    .HasForeignKey(d => d.TitleId)
                    .HasConstraintName("title_lists_ibfk_2");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.Property(e => e.DateCreated)
                    .HasColumnType("datetime")
                    .HasColumnName("date_created")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.Email)
                    .HasMaxLength(255)
                    .HasColumnName("email");

                entity.Property(e => e.Password)
                    .HasMaxLength(255)
                    .HasColumnName("password");

                entity.Property(e => e.UserName)
                    .HasMaxLength(255)
                    .HasColumnName("user_name");

                entity.Property(e => e.UserType)
                    .HasColumnType("enum('standard','admin')")
                    .HasColumnName("user_type")
                    .HasDefaultValue("standard");

            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}

using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AtiDocs.DataModel
{
    public partial class DbFunction : DbContext
    {
        public string DbPath = "";
        public DbFunction()
        {
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            DbPath = $"atidocs.db";
        }

        public virtual DbSet<Article> Articles { get; set; }
        public virtual DbSet<Folder> Folders { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite($"Data Source={DbPath}");
        }

        protected override void OnModelCreating(ModelBuilder model)
        {
            model.Entity<Folder>(entity =>
            {
                entity.HasIndex(p => new { p.Id, p.Slug });

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(256);

                entity.Property(e => e.Slug)
                    .HasColumnName("slug");
            });

            model.Entity<Article>(entity =>
            {
                entity.HasIndex(p => new { p.Id, p.Slug });

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.FolderId)
                    .HasColumnName("folder_id");

                entity.Property(e => e.ChangedAt)
                    .HasColumnName("changedat");

                entity.Property(e => e.Content)
                    .HasColumnName("content");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("createat");

                entity.Property(e => e.Slug)
                    .HasColumnName("slug");

                entity.Property(e => e.Title)
                    .HasColumnName("title")
                    .HasMaxLength(256);

                entity.HasOne(d => d.Folder)
                    .WithMany(p => p.Articles)
                    .HasForeignKey(d => d.FolderId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}

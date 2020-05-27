using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Repositories.Models
{
    public partial class ShapeUp : DbContext
    {
        public ShapeUp()
        {
        }

        public ShapeUp(DbContextOptions<ShapeUp> options)
            : base(options)
        {
        }

        public virtual DbSet<Members> Members { get; set; }
        public virtual DbSet<Point> Point { get; set; }
        public virtual DbSet<Projects> Projects { get; set; }
        public virtual DbSet<ProjectShapeConn> ProjectShapeConn { get; set; }
        public virtual DbSet<Result> Result { get; set; }
        public virtual DbSet<Shapes> Shapes { get; set; }
        public virtual DbSet<Units> Units { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename= G:\\Bnot Elisheva\\ShapeUpDBProject\\DB\\ShapeUp.mdf;Integrated Security=True;Connect Timeout=30");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Members>(entity =>
            {
                entity.Property(e => e.AccountDate)
                    .HasColumnType("date")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.UserPassword)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Point>(entity =>
            {
                entity.Property(e => e.ShapeId).HasColumnName("ShapeID");

                entity.HasOne(d => d.Shape)
                    .WithMany(p => p.Point)
                    .HasForeignKey(d => d.ShapeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Point__ShapeID__6FE99F9F");
            });

            modelBuilder.Entity<Projects>(entity =>
            {
                entity.Property(e => e.DueDate).HasColumnType("date");

                entity.Property(e => e.OrderDate).HasColumnType("date");

                entity.Property(e => e.ProjectDate)
                    .HasColumnType("date")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ProjectName)
                    .HasMaxLength(50)
                    .HasDefaultValueSql("('No Name')");

                entity.Property(e => e.ProjectStatus).HasDefaultValueSql("((0))");

                entity.HasOne(d => d.Member)
                    .WithMany(p => p.Projects)
                    .HasForeignKey(d => d.MemberId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Projects__Member__30F848ED");
            });

            modelBuilder.Entity<ProjectShapeConn>(entity =>
            {
                entity.HasOne(d => d.Project)
                    .WithMany(p => p.ProjectShapeConn)
                    .HasForeignKey(d => d.ProjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ProjectSh__Proje__31EC6D26");

                entity.HasOne(d => d.Shape)
                    .WithMany(p => p.ProjectShapeConn)
                    .HasForeignKey(d => d.ShapeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ProjectSh__Shape__32E0915F");
            });

            modelBuilder.Entity<Result>(entity =>
            {
                entity.Property(e => e.PointOfShapeX).HasColumnName("PointOfShape.X");

                entity.Property(e => e.PointOfShapeY).HasColumnName("PointOfShape.Y");

                entity.Property(e => e.PointOnAreaX).HasColumnName("PointOnArea.X");

                entity.Property(e => e.PointOnAreaY).HasColumnName("PointOnArea.Y");

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.Result)
                    .HasForeignKey(d => d.ProjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Result__ProjectI__05D8E0BE");

                entity.HasOne(d => d.Shape)
                    .WithMany(p => p.Result)
                    .HasForeignKey(d => d.ShapeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Result__ShapeId__04E4BC85");
            });

            modelBuilder.Entity<Shapes>(entity =>
            {
                entity.HasOne(d => d.UnitNavigation)
                    .WithMany(p => p.Shapes)
                    .HasForeignKey(d => d.Unit)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Shapes__Unit__34C8D9D1");
            });

            modelBuilder.Entity<Units>(entity =>
            {
                entity.Property(e => e.UnitName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });
        }
    }
}

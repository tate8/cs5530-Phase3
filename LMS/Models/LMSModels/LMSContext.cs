using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace LMS.Models.LMSModels
{
    public partial class LMSContext : DbContext
    {
        public LMSContext()
        {
        }

        public LMSContext(DbContextOptions<LMSContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Administrator> Administrators { get; set; } = null!;
        public virtual DbSet<Assignment> Assignments { get; set; } = null!;
        public virtual DbSet<AssignmentCategory> AssignmentCategories { get; set; } = null!;
        public virtual DbSet<Class> Classes { get; set; } = null!;
        public virtual DbSet<Course> Courses { get; set; } = null!;
        public virtual DbSet<Department> Departments { get; set; } = null!;
        public virtual DbSet<Enrolled> Enrolleds { get; set; } = null!;
        public virtual DbSet<Professor> Professors { get; set; } = null!;
        public virtual DbSet<Student> Students { get; set; } = null!;
        public virtual DbSet<Submission> Submissions { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySql("name=LMS:LMSConnectionString", Microsoft.EntityFrameworkCore.ServerVersion.Parse("10.11.8-mariadb"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseCollation("latin1_swedish_ci")
                .HasCharSet("latin1");

            modelBuilder.Entity<Administrator>(entity =>
            {
                entity.HasKey(e => e.UId)
                    .HasName("PRIMARY");

                entity.Property(e => e.UId)
                    .HasMaxLength(8)
                    .HasColumnName("uID")
                    .IsFixedLength();

                entity.Property(e => e.Dob).HasColumnName("DOB");

                entity.Property(e => e.FirstName).HasMaxLength(100);

                entity.Property(e => e.LastName).HasMaxLength(100);
            });

            modelBuilder.Entity<Assignment>(entity =>
            {
                entity.HasKey(e => e.AssignId)
                    .HasName("PRIMARY");

                entity.HasIndex(e => new { e.Name, e.CatId }, "Name")
                    .IsUnique();

                entity.HasIndex(e => e.CatId, "catID");

                entity.Property(e => e.AssignId)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("assignID");

                entity.Property(e => e.CatId)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("catID");

                entity.Property(e => e.Contents).HasMaxLength(8192);

                entity.Property(e => e.Due).HasColumnType("datetime");

                entity.Property(e => e.MaxPoints).HasColumnType("int(10) unsigned");

                entity.Property(e => e.Name).HasMaxLength(100);

                entity.HasOne(d => d.Cat)
                    .WithMany(p => p.Assignments)
                    .HasForeignKey(d => d.CatId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Assignments_ibfk_1");
            });

            modelBuilder.Entity<AssignmentCategory>(entity =>
            {
                entity.HasKey(e => e.CatId)
                    .HasName("PRIMARY");

                entity.HasIndex(e => new { e.Name, e.ClassId }, "Name")
                    .IsUnique();

                entity.HasIndex(e => e.ClassId, "classID");

                entity.Property(e => e.CatId)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("catID");

                entity.Property(e => e.ClassId)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("classID");

                entity.Property(e => e.Name).HasMaxLength(100);

                entity.Property(e => e.Weight).HasColumnType("tinyint(3) unsigned");

                entity.HasOne(d => d.Class)
                    .WithMany(p => p.AssignmentCategories)
                    .HasForeignKey(d => d.ClassId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("AssignmentCategories_ibfk_1");
            });

            modelBuilder.Entity<Class>(entity =>
            {
                entity.HasIndex(e => new { e.CourseId, e.Season, e.Year }, "courseID")
                    .IsUnique();

                entity.HasIndex(e => e.UId, "uID");

                entity.Property(e => e.ClassId)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("classID");

                entity.Property(e => e.CourseId)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("courseID");

                entity.Property(e => e.EndTime).HasColumnType("time");

                entity.Property(e => e.Location).HasMaxLength(100);

                entity.Property(e => e.Season).HasMaxLength(6);

                entity.Property(e => e.StartTime).HasColumnType("time");

                entity.Property(e => e.UId)
                    .HasMaxLength(8)
                    .HasColumnName("uID")
                    .IsFixedLength();

                entity.Property(e => e.Year).HasColumnType("int(10) unsigned");

                entity.HasOne(d => d.Course)
                    .WithMany(p => p.Classes)
                    .HasForeignKey(d => d.CourseId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Classes_ibfk_1");

                entity.HasOne(d => d.UIdNavigation)
                    .WithMany(p => p.Classes)
                    .HasForeignKey(d => d.UId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Classes_ibfk_2");
            });

            modelBuilder.Entity<Course>(entity =>
            {
                entity.HasIndex(e => new { e.DeptAbrv, e.CNum }, "DeptAbrv")
                    .IsUnique();

                entity.Property(e => e.CourseId)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("courseID");

                entity.Property(e => e.CName)
                    .HasMaxLength(100)
                    .HasColumnName("cName");

                entity.Property(e => e.CNum)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("cNum");

                entity.Property(e => e.DeptAbrv).HasMaxLength(4);

                entity.HasOne(d => d.DeptAbrvNavigation)
                    .WithMany(p => p.Courses)
                    .HasForeignKey(d => d.DeptAbrv)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Courses_ibfk_1");
            });

            modelBuilder.Entity<Department>(entity =>
            {
                entity.HasKey(e => e.DeptAbrv)
                    .HasName("PRIMARY");

                entity.Property(e => e.DeptAbrv).HasMaxLength(4);

                entity.Property(e => e.Name).HasMaxLength(100);
            });

            modelBuilder.Entity<Enrolled>(entity =>
            {
                entity.HasKey(e => e.EId)
                    .HasName("PRIMARY");

                entity.ToTable("Enrolled");

                entity.HasIndex(e => e.ClassId, "classID");

                entity.HasIndex(e => new { e.UId, e.ClassId }, "uID")
                    .IsUnique();

                entity.Property(e => e.EId)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("eID");

                entity.Property(e => e.ClassId)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("classID");

                entity.Property(e => e.Grade).HasMaxLength(2);

                entity.Property(e => e.UId)
                    .HasMaxLength(8)
                    .HasColumnName("uID")
                    .IsFixedLength();

                entity.HasOne(d => d.Class)
                    .WithMany(p => p.Enrolleds)
                    .HasForeignKey(d => d.ClassId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Enrolled_ibfk_2");

                entity.HasOne(d => d.UIdNavigation)
                    .WithMany(p => p.Enrolleds)
                    .HasForeignKey(d => d.UId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Enrolled_ibfk_1");
            });

            modelBuilder.Entity<Professor>(entity =>
            {
                entity.HasKey(e => e.UId)
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.DeptAbrv, "DeptAbrv");

                entity.Property(e => e.UId)
                    .HasMaxLength(8)
                    .HasColumnName("uID")
                    .IsFixedLength();

                entity.Property(e => e.DeptAbrv).HasMaxLength(4);

                entity.Property(e => e.Dob).HasColumnName("DOB");

                entity.Property(e => e.FirstName).HasMaxLength(100);

                entity.Property(e => e.LastName).HasMaxLength(100);

                entity.HasOne(d => d.DeptAbrvNavigation)
                    .WithMany(p => p.Professors)
                    .HasForeignKey(d => d.DeptAbrv)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Professors_ibfk_2");
            });

            modelBuilder.Entity<Student>(entity =>
            {
                entity.HasKey(e => e.UId)
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.DeptAbrv, "DeptAbrv");

                entity.Property(e => e.UId)
                    .HasMaxLength(8)
                    .HasColumnName("uID")
                    .IsFixedLength();

                entity.Property(e => e.DeptAbrv).HasMaxLength(4);

                entity.Property(e => e.Dob).HasColumnName("DOB");

                entity.Property(e => e.FirstName).HasMaxLength(100);

                entity.Property(e => e.LastName).HasMaxLength(100);

                entity.HasOne(d => d.DeptAbrvNavigation)
                    .WithMany(p => p.Students)
                    .HasForeignKey(d => d.DeptAbrv)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Students_ibfk_2");
            });

            modelBuilder.Entity<Submission>(entity =>
            {
                entity.HasIndex(e => new { e.AssignId, e.UId }, "assignID")
                    .IsUnique();

                entity.HasIndex(e => e.UId, "uID");

                entity.Property(e => e.SubmissionId)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("submissionID");

                entity.Property(e => e.Answer).HasMaxLength(8192);

                entity.Property(e => e.AssignId)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("assignID");

                entity.Property(e => e.Score).HasColumnType("int(10) unsigned");

                entity.Property(e => e.SubmitTime).HasColumnType("datetime");

                entity.Property(e => e.UId)
                    .HasMaxLength(8)
                    .HasColumnName("uID")
                    .IsFixedLength();

                entity.HasOne(d => d.Assign)
                    .WithMany(p => p.Submissions)
                    .HasForeignKey(d => d.AssignId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Submissions_ibfk_1");

                entity.HasOne(d => d.UIdNavigation)
                    .WithMany(p => p.Submissions)
                    .HasForeignKey(d => d.UId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Submissions_ibfk_2");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}

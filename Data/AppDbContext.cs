using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SIMS.API.Models;

namespace SIMS.API.Data;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Attendance> Attendances { get; set; }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<CourseStudent> CourseStudents { get; set; }

    public virtual DbSet<Grade> Grades { get; set; }

    public virtual DbSet<Schedule> Schedules { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=ConnectionStrings:DefaultConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Attendance>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Attendan__3214EC074739C8C6");

            entity.HasOne(d => d.Course).WithMany(p => p.Attendances).HasConstraintName("FK_Attendance_Course");

            entity.HasOne(d => d.Student).WithMany(p => p.Attendances)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Attendance_Student");
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Courses__3214EC074E4882A9");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Teacher).WithMany(p => p.Courses)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Courses_Teacher");
        });

        modelBuilder.Entity<CourseStudent>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CourseSt__3214EC07A1C8D8CA");

            entity.Property(e => e.EnrolledAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Course).WithMany(p => p.CourseStudents).HasConstraintName("FK_CourseStudents_Course");

            entity.HasOne(d => d.Student).WithMany(p => p.CourseStudents)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CourseStudents_Student");
        });

        modelBuilder.Entity<Grade>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Grades__3214EC076232D52A");

            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Course).WithMany(p => p.Grades).HasConstraintName("FK_Grades_Course");

            entity.HasOne(d => d.Student).WithMany(p => p.Grades)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Grades_Student");
        });

        modelBuilder.Entity<Schedule>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Schedule__3214EC07803BDA6F");

            entity.HasOne(d => d.Course).WithMany(p => p.Schedules).HasConstraintName("FK_Schedules_Course");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC079843C7C3");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

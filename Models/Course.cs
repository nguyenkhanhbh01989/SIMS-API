using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SIMS.API.Models;

public partial class Course
{
    [Key]
    public int Id { get; set; }

    [StringLength(100)]
    public string Name { get; set; } = null!;

    [StringLength(20)]
    public string Semester { get; set; } = null!;

    public int? TeacherId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    [InverseProperty("Course")]
    public virtual ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();

    [InverseProperty("Course")]
    public virtual ICollection<CourseStudent> CourseStudents { get; set; } = new List<CourseStudent>();

    [InverseProperty("Course")]
    public virtual ICollection<Grade> Grades { get; set; } = new List<Grade>();

    [InverseProperty("Course")]
    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();

    [ForeignKey("TeacherId")]
    [InverseProperty("Courses")]
    public virtual User? Teacher { get; set; }
}

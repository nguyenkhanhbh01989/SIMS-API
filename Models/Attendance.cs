using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SIMS.API.Models;

[Table("Attendance")]
[Index("CourseId", "StudentId", "AttendanceDate", Name = "UQ_Attendance", IsUnique = true)]
public partial class Attendance
{
    [Key]
    public int Id { get; set; }

    public int CourseId { get; set; }

    public int StudentId { get; set; }

    public DateOnly AttendanceDate { get; set; }

    public bool IsPresent { get; set; }

    [StringLength(255)]
    public string? Note { get; set; }

    [ForeignKey("CourseId")]
    [InverseProperty("Attendances")]
    public virtual Course Course { get; set; } = null!;

    [ForeignKey("StudentId")]
    [InverseProperty("Attendances")]
    public virtual User Student { get; set; } = null!;
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SIMS.API.Models;

[Index("CourseId", "StudentId", Name = "UQ_Grades_Course_Student", IsUnique = true)]
public partial class Grade
{
    [Key]
    public int Id { get; set; }

    public int CourseId { get; set; }

    public int StudentId { get; set; }

    public double? Midterm { get; set; }

    public double? Final { get; set; }

    public double? Other { get; set; }

    public double? Total { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime UpdatedAt { get; set; }

    [ForeignKey("CourseId")]
    [InverseProperty("Grades")]
    public virtual Course Course { get; set; } = null!;

    [ForeignKey("StudentId")]
    [InverseProperty("Grades")]
    public virtual User Student { get; set; } = null!;
}

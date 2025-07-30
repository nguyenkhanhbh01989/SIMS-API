using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SIMS.API.Models;

[Index("CourseId", "StudentId", Name = "UQ_Course_Student", IsUnique = true)]
public partial class CourseStudent
{
    [Key]
    public int Id { get; set; }

    public int CourseId { get; set; }

    public int StudentId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime EnrolledAt { get; set; }

    [ForeignKey("CourseId")]
    [InverseProperty("CourseStudents")]
    public virtual Course Course { get; set; } = null!;

    [ForeignKey("StudentId")]
    [InverseProperty("CourseStudents")]
    public virtual User Student { get; set; } = null!;
}

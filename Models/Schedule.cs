using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SIMS.API.Models;

[Index("DayOfWeek", "StartTime", "Location", Name = "UQ_Schedule_Location_Time", IsUnique = true)]
public partial class Schedule
{
    [Key]
    public int Id { get; set; }

    public int CourseId { get; set; }

    public int DayOfWeek { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    [StringLength(100)]
    public string Location { get; set; } = null!;

    [ForeignKey("CourseId")]
    [InverseProperty("Schedules")]
    public virtual Course Course { get; set; } = null!;
}

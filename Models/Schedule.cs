using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SIMS.API.Models;

[Index("CourseId", "DayOfWeek", "SlotNumber", Name = "UQ_Schedule_Course_Slot", IsUnique = true)]
[Index("DayOfWeek", "SlotNumber", "Location", Name = "UQ_Schedule_Location_Slot", IsUnique = true)]
public partial class Schedule
{
    [Key]
    public int Id { get; set; }

    public int CourseId { get; set; }

    public int DayOfWeek { get; set; }

    public int SlotNumber { get; set; }

    [StringLength(100)]
    public string Location { get; set; } = null!;

    [ForeignKey("CourseId")]
    [InverseProperty("Schedules")]
    public virtual Course Course { get; set; } = null!;
}

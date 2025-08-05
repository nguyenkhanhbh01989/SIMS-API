using System.ComponentModel.DataAnnotations;

namespace SIMS.API.DTOs.Schedule
{
    public class ScheduleUpdateDto
    {
        [Required]
        [Range(0, 6, ErrorMessage = "Day of week must be between 0 (Sunday) and 6 (Saturday).")]
        public int DayOfWeek { get; set; }

        [Required]
        public TimeOnly StartTime { get; set; }

        [Required]
        public TimeOnly EndTime { get; set; }

        [Required]
        [StringLength(100)]
        public required string Location { get; set; }
    }
}
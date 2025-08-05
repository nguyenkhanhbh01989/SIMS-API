
using System.ComponentModel.DataAnnotations;

namespace SIMS.API.DTOs.Schedule
{
    public class ScheduleUpdateDto
    {
        [Required]
        [Range(0, 6)]
        public int DayOfWeek { get; set; }

        [Required]
        [Range(1, 7)]
        public int SlotNumber { get; set; }

        [Required]
        [StringLength(100)]
        public required string Location { get; set; }
    }
}
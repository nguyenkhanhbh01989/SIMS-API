using System.ComponentModel.DataAnnotations;

namespace SIMS.API.DTOs.Attendance
{
    public class StudentAttendanceDto
    {
        [Required]
        public int StudentId { get; set; }

        [Required]
        public bool IsPresent { get; set; }

        [StringLength(255)]
        public string? Note { get; set; }
    }
}
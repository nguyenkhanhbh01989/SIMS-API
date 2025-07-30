using System.ComponentModel.DataAnnotations;

namespace SIMS.API.DTOs.Enrollment
{
    public class EnrollmentRequestDto
    {
        [Required]
        public int CourseId { get; set; }

        [Required]
        public int StudentId { get; set; }
    }
}
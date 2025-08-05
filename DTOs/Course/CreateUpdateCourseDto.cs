using System.ComponentModel.DataAnnotations;

namespace SIMS.API.DTOs.Course
{
    public class CreateUpdateCourseDto
    {
        [Required]
        [StringLength(100)]
        public required string Name { get; set; }

        [Required]
        [StringLength(20)]
        public required string Semester { get; set; }

        public int? TeacherId { get; set; }

        // Các thuộc tính này là nullable, khớp với database
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
    }
}
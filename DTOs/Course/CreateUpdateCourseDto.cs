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

        // ID của giáo viên phụ trách. Có thể là null.
        public int? TeacherId { get; set; }
    }
}
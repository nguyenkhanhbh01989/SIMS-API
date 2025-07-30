using SIMS.API.DTOs.Course;
using SIMS.API.Models;
using System.ComponentModel.DataAnnotations;

namespace SIMS.API.DTOs.Course
{
    public class CourseViewDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Semester { get; set; }
        public int? TeacherId { get; set; }
        public string? TeacherName { get; set; } // Thêm tên giáo viên để hiển thị
        public DateTime CreatedAt { get; set; }
    }
}
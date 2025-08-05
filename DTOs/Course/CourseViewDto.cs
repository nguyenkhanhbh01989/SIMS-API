namespace SIMS.API.DTOs.Course
{
    public class CourseViewDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Semester { get; set; }
        public int? TeacherId { get; set; }
        public string? TeacherName { get; set; }
        public DateTime CreatedAt { get; set; }

        // Thêm 2 thuộc tính mới
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
    }
}
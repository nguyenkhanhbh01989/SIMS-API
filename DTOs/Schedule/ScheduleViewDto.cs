namespace SIMS.API.DTOs.Schedule
{
    public class ScheduleViewDto
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public required string CourseName { get; set; }
        public int? TeacherId { get; set; }
        public string? TeacherName { get; set; }
        public required string DayOfWeek { get; set; } // Hiển thị dạng chữ (Thứ Hai, Thứ Ba...)
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public required string Location { get; set; }
    }
}
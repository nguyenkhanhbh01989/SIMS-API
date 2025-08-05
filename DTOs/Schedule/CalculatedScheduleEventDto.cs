
namespace SIMS.API.DTOs.Schedule
{
    public class CalculatedScheduleEventDto
    {
        public int CourseId { get; set; }
        public required string CourseName { get; set; }
        public string? TeacherName { get; set; }
        public DateOnly SpecificDate { get; set; }
        public required string DayOfWeek { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public required string Location { get; set; }
    }
}
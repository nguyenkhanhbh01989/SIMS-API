using SIMS.API.DTOs.Schedule;
using SIMS.API.Models;

namespace SIMS.API.DTOs.Schedule
{
    public class ScheduleViewDto
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public required string CourseName { get; set; }
        public int? TeacherId { get; set; }
        public string? TeacherName { get; set; }
        public required string DayOfWeek { get; set; }
        public int SlotNumber { get; set; } // Thêm SlotNumber
        public TimeOnly StartTime { get; set; } // Giữ lại để hiển thị
        public TimeOnly EndTime { get; set; }   // Giữ lại để hiển thị
        public required string Location { get; set; }
    }
}
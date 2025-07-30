namespace SIMS.API.DTOs.Attendance
{
    public class AttendanceRecordDto
    {
        public int StudentId { get; set; }
        public required string StudentName { get; set; }
        public DateOnly AttendanceDate { get; set; }
        public bool IsPresent { get; set; }
        public string? Note { get; set; }
    }
}
namespace SIMS.API.DTOs.Enrollment
{
    public class EnrollmentViewDto
    {
        public int CourseId { get; set; }
        public required string CourseName { get; set; }
        public int StudentId { get; set; }
        public required string StudentName { get; set; }
        public required string StudentEmail { get; set; }
        public DateTime EnrolledAt { get; set; }
    }
}
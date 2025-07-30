namespace SIMS.API.DTOs.Teacher
{
    public class StudentInCourseDto
    {
        public int StudentId { get; set; }
        public required string FullName { get; set; }
        public required string Email { get; set; }
        public DateTime EnrolledAt { get; set; }
    }
}
namespace SIMS.API.DTOs.Teacher
{
    public class GradeViewDto
    {
        public int StudentId { get; set; }
        public required string StudentName { get; set; }
        public double? Midterm { get; set; }
        public double? Final { get; set; }
        public double? Other { get; set; }
        public double? Total { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
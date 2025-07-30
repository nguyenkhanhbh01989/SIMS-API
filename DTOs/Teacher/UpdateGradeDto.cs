namespace SIMS.API.DTOs.Teacher
{
    public class UpdateGradeDto
    {
        public double? Midterm { get; set; }
        public double? Final { get; set; }
        public double? Other { get; set; }
        public double? Total { get; set; }
    }
}
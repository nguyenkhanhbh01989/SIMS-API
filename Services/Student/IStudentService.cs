using SIMS.API.DTOs.Course;
using SIMS.API.DTOs.Teacher;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SIMS.API.Services.Student
{
    public interface IStudentService
    {
        Task<IEnumerable<CourseViewDto>> GetMyCoursesAsync(int studentId);
        Task<GradeViewDto?> GetMyGradeForCourseAsync(int studentId, int courseId);
    }
}
using SIMS.API.DTOs.Enrollment;
using SIMS.API.DTOs.Teacher;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SIMS.API.Services.Enrollment
{
    public interface IEnrollmentService
    {
        Task<EnrollmentViewDto> EnrollStudentAsync(EnrollmentRequestDto requestDto);
        Task<bool> UnenrollStudentAsync(int courseId, int studentId);
        Task<IEnumerable<EnrollmentViewDto>> GetAllEnrollmentsAsync();
        Task<IEnumerable<StudentInCourseDto>> GetStudentsInCourseAsync(int courseId);
    }
}
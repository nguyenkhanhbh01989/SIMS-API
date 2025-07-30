using SIMS.API.DTOs.Course;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SIMS.API.Services.Course
{
    public interface ICourseService
    {
        Task<CourseViewDto> CreateCourseAsync(CreateUpdateCourseDto courseDto);
        Task<IEnumerable<CourseViewDto>> GetAllCoursesAsync();
        Task<CourseViewDto?> GetCourseByIdAsync(int id);
        Task<CourseViewDto> UpdateCourseAsync(int id, CreateUpdateCourseDto courseDto);
        Task<bool> DeleteCourseAsync(int id);
    }
}
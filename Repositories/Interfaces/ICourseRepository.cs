using SIMS.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SIMS.API.Repositories.Interfaces
{
    public interface ICourseRepository
    {
        Task<Course> CreateAsync(Course course);
        Task<IEnumerable<Course>> GetAllAsync();
        Task<Course?> GetByIdAsync(int id);
        Task<Course> UpdateAsync(Course course);
        Task DeleteAsync(int id);

        // --- PHƯƠNG THỨC MỚI CHO GIÁO VIÊN ---
        Task<IEnumerable<Course>> GetByTeacherIdAsync(int teacherId);
        Task<IEnumerable<CourseStudent>> GetEnrollmentsByCourseIdAsync(int courseId);
    }
}
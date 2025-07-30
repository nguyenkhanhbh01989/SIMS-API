using SIMS.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SIMS.API.Repositories.Interfaces
{
    public interface IEnrollmentRepository
    {
        Task<CourseStudent> EnrollAsync(CourseStudent enrollment);
        Task UnenrollAsync(int courseId, int studentId);
        Task<CourseStudent?> GetEnrollmentAsync(int courseId, int studentId);
        Task<IEnumerable<CourseStudent>> GetAllEnrollmentsAsync();
        Task<IEnumerable<CourseStudent>> GetEnrollmentsByStudentIdAsync(int studentId);
    }
}
using SIMS.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SIMS.API.Repositories.Interfaces
{
    public interface IGradeRepository
    {
        Task<IEnumerable<Grade>> GetGradesByCourseIdAsync(int courseId);
        Task<Grade?> GetGradeAsync(int courseId, int studentId);
        Task<Grade> AddOrUpdateGradeAsync(Grade grade);
    }
}
using SIMS.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SIMS.API.Repositories.Interfaces
{
    public interface IScheduleRepository
    {
        Task<Schedule> CreateAsync(Schedule schedule);
        Task<Schedule?> GetByIdAsync(int id);
        Task<Schedule> UpdateAsync(Schedule schedule);
        Task DeleteAsync(int id);
        Task<IEnumerable<Schedule>> GetByTeacherIdAsync(int teacherId);
        Task<IEnumerable<Schedule>> GetByStudentIdAsync(int studentId);
        Task<IEnumerable<Schedule>> GetAllAsync();
    }
}
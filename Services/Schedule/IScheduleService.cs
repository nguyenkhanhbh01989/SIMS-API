using SIMS.API.DTOs.Schedule;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SIMS.API.Services.Schedule
{
    public interface IScheduleService
    {
        Task<ScheduleViewDto> CreateScheduleAsync(ScheduleCreateDto scheduleDto);
        Task<ScheduleViewDto> UpdateScheduleAsync(int id, ScheduleUpdateDto scheduleDto);
        Task<bool> DeleteScheduleAsync(int id);
        Task<IEnumerable<ScheduleViewDto>> GetTeacherScheduleAsync(int teacherId);
        Task<IEnumerable<ScheduleViewDto>> GetStudentScheduleAsync(int studentId);
        Task<IEnumerable<ScheduleViewDto>> GetAllSchedulesAsync();
    }
}
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

        // Đảm bảo chỉ có phiên bản này tồn tại
        Task<IEnumerable<CalculatedScheduleEventDto>> GetTeacherScheduleAsync(int teacherId);

        // Đảm bảo chỉ có phiên bản này tồn tại
        Task<IEnumerable<CalculatedScheduleEventDto>> GetStudentScheduleAsync(int studentId);

        Task<IEnumerable<ScheduleViewDto>> GetAllSchedulesAsync();
    }
}
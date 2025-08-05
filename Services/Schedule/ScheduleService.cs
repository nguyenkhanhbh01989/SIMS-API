using SIMS.API.DTOs.Schedule;
using SIMS.API.Models;
using SIMS.API.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SIMS.API.Services.Schedule
{
    public class ScheduleService : IScheduleService
    {
        private readonly IScheduleRepository _scheduleRepository;
        private readonly ICourseRepository _courseRepository;

        public ScheduleService(IScheduleRepository scheduleRepository, ICourseRepository courseRepository)
        {
            _scheduleRepository = scheduleRepository;
            _courseRepository = courseRepository;
        }

        public async Task<ScheduleViewDto> CreateScheduleAsync(ScheduleCreateDto scheduleDto)
        {
            var course = await _courseRepository.GetByIdAsync(scheduleDto.CourseId);
            if (course == null)
            {
                throw new ApplicationException("Môn học không tồn tại.");
            }

            if (course.TeacherId.HasValue)
            {
                await CheckForConflict(course.TeacherId.Value, 0, scheduleDto.DayOfWeek, scheduleDto.StartTime, scheduleDto.EndTime);
            }

            var newSchedule = new Models.Schedule
            {
                CourseId = scheduleDto.CourseId,
                DayOfWeek = scheduleDto.DayOfWeek,
                StartTime = scheduleDto.StartTime,
                EndTime = scheduleDto.EndTime,
                Location = scheduleDto.Location
            };

            var createdSchedule = await _scheduleRepository.CreateAsync(newSchedule);
            var result = await _scheduleRepository.GetByIdAsync(createdSchedule.Id);
            return MapToViewDto(result!);
        }

        public async Task<ScheduleViewDto> UpdateScheduleAsync(int id, ScheduleUpdateDto scheduleDto)
        {
            var scheduleToUpdate = await _scheduleRepository.GetByIdAsync(id);
            if (scheduleToUpdate == null)
            {
                throw new ApplicationException("Không tìm thấy lịch học để cập nhật.");
            }

            if (scheduleToUpdate.Course.TeacherId.HasValue)
            {
                await CheckForConflict(scheduleToUpdate.Course.TeacherId.Value, id, scheduleDto.DayOfWeek, scheduleDto.StartTime, scheduleDto.EndTime);
            }

            scheduleToUpdate.DayOfWeek = scheduleDto.DayOfWeek;
            scheduleToUpdate.StartTime = scheduleDto.StartTime;
            scheduleToUpdate.EndTime = scheduleDto.EndTime;
            scheduleToUpdate.Location = scheduleDto.Location;

            var updatedSchedule = await _scheduleRepository.UpdateAsync(scheduleToUpdate);
            return MapToViewDto(updatedSchedule);
        }

        public async Task<bool> DeleteScheduleAsync(int id)
        {
            var schedule = await _scheduleRepository.GetByIdAsync(id);
            if (schedule == null) return false;
            await _scheduleRepository.DeleteAsync(id);
            return true;
        }

        public async Task<IEnumerable<ScheduleViewDto>> GetTeacherScheduleAsync(int teacherId)
        {
            var schedules = await _scheduleRepository.GetByTeacherIdAsync(teacherId);
            return schedules.Select(MapToViewDto);
        }

        public async Task<IEnumerable<ScheduleViewDto>> GetStudentScheduleAsync(int studentId)
        {
            var schedules = await _scheduleRepository.GetByStudentIdAsync(studentId);
            return schedules.Select(MapToViewDto);
        }

        public async Task<IEnumerable<ScheduleViewDto>> GetAllSchedulesAsync()
        {
            var schedules = await _scheduleRepository.GetAllAsync();
            return schedules.Select(MapToViewDto);
        }

        // Phương thức helper để kiểm tra xung đột lịch của giáo viên
        private async Task CheckForConflict(int teacherId, int currentScheduleId, int dayOfWeek, TimeOnly startTime, TimeOnly endTime)
        {
            var teacherSchedules = await _scheduleRepository.GetByTeacherIdAsync(teacherId);
            var conflictingSchedule = teacherSchedules.FirstOrDefault(s =>
                s.Id != currentScheduleId && // Bỏ qua chính lịch đang sửa
                s.DayOfWeek == dayOfWeek &&
                startTime < s.EndTime &&
                endTime > s.StartTime);

            if (conflictingSchedule != null)
            {
                throw new ApplicationException(
                    $"Xung đột lịch! Giáo viên đã có lịch dạy môn '{conflictingSchedule.Course.Name}' " +
                    $"từ {conflictingSchedule.StartTime} đến {conflictingSchedule.EndTime} vào ngày này.");
            }
        }

        // Phương thức helper để map sang DTO
        private ScheduleViewDto MapToViewDto(Models.Schedule schedule)
        {
            return new ScheduleViewDto
            {
                Id = schedule.Id,
                CourseId = schedule.CourseId,
                CourseName = schedule.Course.Name,
                TeacherId = schedule.Course.TeacherId,
                TeacherName = schedule.Course.Teacher?.FullName,
                DayOfWeek = ConvertDayOfWeekToString(schedule.DayOfWeek),
                StartTime = schedule.StartTime,
                EndTime = schedule.EndTime,
                Location = schedule.Location
            };
        }

        private string ConvertDayOfWeekToString(int day)
        {
            return day switch
            {
                0 => "Sunday",
                1 => "Monday",
                2 => "Tuesday",
                3 => "Wednesday",
                4 => "Thursday",
                5 => "Friday",
                6 => "Saturday",
                _ => "Unknown"
            };

        }
    }
}
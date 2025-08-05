using SIMS.API.DTOs.Schedule;
using SIMS.API.Helpers;
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
            if (course == null) throw new ApplicationException("Môn học không tồn tại.");
            if (TimeSlotHelper.GetTimeSlot(scheduleDto.SlotNumber) == null) throw new ApplicationException("Ca học không hợp lệ.");

            if (course.TeacherId.HasValue)
            {
                await CheckForConflict(course.TeacherId.Value, 0, scheduleDto.DayOfWeek, scheduleDto.SlotNumber);
            }

            var newSchedule = new Models.Schedule
            {
                CourseId = scheduleDto.CourseId,
                DayOfWeek = scheduleDto.DayOfWeek,
                SlotNumber = scheduleDto.SlotNumber,
                Location = scheduleDto.Location
            };

            var createdSchedule = await _scheduleRepository.CreateAsync(newSchedule);
            var result = await _scheduleRepository.GetByIdAsync(createdSchedule.Id);
            return MapToViewDto(result!);
        }

        public async Task<ScheduleViewDto> UpdateScheduleAsync(int id, ScheduleUpdateDto scheduleDto)
        {
            var scheduleToUpdate = await _scheduleRepository.GetByIdAsync(id);
            if (scheduleToUpdate == null) throw new ApplicationException("Không tìm thấy lịch học.");
            if (TimeSlotHelper.GetTimeSlot(scheduleDto.SlotNumber) == null) throw new ApplicationException("Ca học không hợp lệ.");

            if (scheduleToUpdate.Course.TeacherId.HasValue)
            {
                await CheckForConflict(scheduleToUpdate.Course.TeacherId.Value, id, scheduleDto.DayOfWeek, scheduleDto.SlotNumber);
            }

            scheduleToUpdate.DayOfWeek = scheduleDto.DayOfWeek;
            scheduleToUpdate.SlotNumber = scheduleDto.SlotNumber;
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

        public async Task<IEnumerable<CalculatedScheduleEventDto>> GetTeacherScheduleAsync(int teacherId)
        {
            var schedules = await _scheduleRepository.GetByTeacherIdAsync(teacherId);
            return CalculateEventsFromSchedules(schedules);
        }

        public async Task<IEnumerable<CalculatedScheduleEventDto>> GetStudentScheduleAsync(int studentId)
        {
            var schedules = await _scheduleRepository.GetByStudentIdAsync(studentId);
            return CalculateEventsFromSchedules(schedules);
        }

        public async Task<IEnumerable<ScheduleViewDto>> GetAllSchedulesAsync()
        {
            var schedules = await _scheduleRepository.GetAllAsync();
            return schedules.Select(MapToViewDto);
        }

        private async Task CheckForConflict(int teacherId, int currentScheduleId, int dayOfWeek, int slotNumber)
        {
            var teacherSchedules = await _scheduleRepository.GetByTeacherIdAsync(teacherId);
            var conflictingSchedule = teacherSchedules.FirstOrDefault(s =>
                s.Id != currentScheduleId &&
                s.DayOfWeek == dayOfWeek &&
                s.SlotNumber == slotNumber);

            if (conflictingSchedule != null)
            {
                throw new ApplicationException($"Xung đột lịch! Giáo viên đã có lịch dạy môn '{conflictingSchedule.Course.Name}' vào ca {slotNumber} ngày này.");
            }
        }

        private List<CalculatedScheduleEventDto> CalculateEventsFromSchedules(IEnumerable<Models.Schedule> schedules)
        {
            var events = new List<CalculatedScheduleEventDto>();
            foreach (var schedule in schedules)
            {
                if (!schedule.Course.StartDate.HasValue || !schedule.Course.EndDate.HasValue) continue;

                var timeSlot = TimeSlotHelper.GetTimeSlot(schedule.SlotNumber);
                if (timeSlot == null) continue;

                for (var day = schedule.Course.StartDate.Value; day <= schedule.Course.EndDate.Value; day = day.AddDays(1))
                {
                    if ((int)day.DayOfWeek == schedule.DayOfWeek)
                    {
                        events.Add(new CalculatedScheduleEventDto
                        {
                            CourseId = schedule.CourseId,
                            CourseName = schedule.Course.Name,
                            TeacherName = schedule.Course.Teacher?.FullName,
                            SpecificDate = day,
                            DayOfWeek = ConvertDayOfWeekToString(schedule.DayOfWeek),
                            StartTime = timeSlot.StartTime,
                            EndTime = timeSlot.EndTime,
                            Location = schedule.Location
                        });
                    }
                }
            }
            return events.OrderBy(e => e.SpecificDate).ThenBy(e => e.StartTime).ToList();
        }

        private ScheduleViewDto MapToViewDto(Models.Schedule schedule)
        {
            var timeSlot = TimeSlotHelper.GetTimeSlot(schedule.SlotNumber);
            return new ScheduleViewDto
            {
                Id = schedule.Id,
                CourseId = schedule.CourseId,
                CourseName = schedule.Course.Name,
                TeacherId = schedule.Course.TeacherId,
                TeacherName = schedule.Course.Teacher?.FullName,
                DayOfWeek = ConvertDayOfWeekToString(schedule.DayOfWeek),
                SlotNumber = schedule.SlotNumber,
                StartTime = timeSlot?.StartTime ?? default,
                EndTime = timeSlot?.EndTime ?? default,
                Location = schedule.Location
            };
        }

        private string ConvertDayOfWeekToString(int day)
        {
            return ((DayOfWeek)day).ToString(); // Cách đơn giản hơn
        }
    }
}
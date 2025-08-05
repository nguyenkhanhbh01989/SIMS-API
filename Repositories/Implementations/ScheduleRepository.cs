using Microsoft.EntityFrameworkCore;
using SIMS.API.Data;
using SIMS.API.Models;
using SIMS.API.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SIMS.API.Repositories.Implementations
{
    public class ScheduleRepository : IScheduleRepository
    {
        private readonly AppDbContext _context;

        public ScheduleRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Schedule> CreateAsync(Schedule schedule)
        {
            await _context.Schedules.AddAsync(schedule);
            await _context.SaveChangesAsync();
            return schedule;
        }

        public async Task<Schedule?> GetByIdAsync(int id)
        {
            return await _context.Schedules
                .Include(s => s.Course)
                .ThenInclude(c => c.Teacher)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Schedule> UpdateAsync(Schedule schedule)
        {
            _context.Schedules.Update(schedule);
            await _context.SaveChangesAsync();
            return schedule;
        }

        public async Task DeleteAsync(int id)
        {
            var schedule = await _context.Schedules.FindAsync(id);
            if (schedule != null)
            {
                _context.Schedules.Remove(schedule);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Schedule>> GetByTeacherIdAsync(int teacherId)
        {
            return await _context.Schedules
                .Where(s => s.Course.TeacherId == teacherId)
                .Include(s => s.Course)
                .OrderBy(s => s.DayOfWeek).ThenBy(s => s.StartTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Schedule>> GetByStudentIdAsync(int studentId)
        {
            // Lấy các CourseId mà sinh viên đã đăng ký
            var courseIds = await _context.CourseStudents
                .Where(cs => cs.StudentId == studentId)
                .Select(cs => cs.CourseId)
                .ToListAsync();

            return await _context.Schedules
                .Where(s => courseIds.Contains(s.CourseId))
                .Include(s => s.Course)
                .ThenInclude(c => c.Teacher)
                .OrderBy(s => s.DayOfWeek).ThenBy(s => s.StartTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Schedule>> GetAllAsync()
        {
            return await _context.Schedules
                .Include(s => s.Course)
                .ThenInclude(c => c.Teacher)
                .OrderBy(s => s.Course.Name).ThenBy(s => s.DayOfWeek).ThenBy(s => s.StartTime)
                .ToListAsync();
        }
    }
}
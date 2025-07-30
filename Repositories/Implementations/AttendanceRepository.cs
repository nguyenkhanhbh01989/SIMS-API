using Microsoft.EntityFrameworkCore;
using SIMS.API.Data;
using SIMS.API.Models;
using SIMS.API.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SIMS.API.Repositories.Implementations
{
    public class AttendanceRepository : IAttendanceRepository
    {
        private readonly AppDbContext _context;

        public AttendanceRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Attendance>> GetAttendanceForCourseAsync(int courseId, DateOnly? date)
        {
            var query = _context.Attendances
                .Include(a => a.Student)
                .Where(a => a.CourseId == courseId);

            if (date.HasValue)
            {
                query = query.Where(a => a.AttendanceDate == date.Value);
            }

            return await query.OrderBy(a => a.AttendanceDate).ThenBy(a => a.Student.FullName).ToListAsync();
        }

        public async Task UpsertAttendanceAsync(IEnumerable<Attendance> attendanceRecords)
        {
            // Lấy các bản ghi hiện có để so sánh
            var dates = attendanceRecords.Select(r => r.AttendanceDate).Distinct();
            var studentIds = attendanceRecords.Select(r => r.StudentId).Distinct();
            var courseId = attendanceRecords.FirstOrDefault()?.CourseId;

            var existingRecords = await _context.Attendances
                .Where(a => a.CourseId == courseId &&
                            dates.Contains(a.AttendanceDate) &&
                            studentIds.Contains(a.StudentId))
                .ToListAsync();

            var recordsToAdd = new List<Attendance>();

            foreach (var record in attendanceRecords)
            {
                var existing = existingRecords.FirstOrDefault(e => e.StudentId == record.StudentId && e.AttendanceDate == record.AttendanceDate);
                if (existing != null)
                {
                    // Cập nhật bản ghi đã có
                    existing.IsPresent = record.IsPresent;
                    existing.Note = record.Note;
                }
                else
                {
                    // Thêm bản ghi mới
                    recordsToAdd.Add(record);
                }
            }

            if (recordsToAdd.Any())
            {
                await _context.Attendances.AddRangeAsync(recordsToAdd);
            }

            await _context.SaveChangesAsync();
        }
    }
}
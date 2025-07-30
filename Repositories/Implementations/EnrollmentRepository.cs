using Microsoft.EntityFrameworkCore;
using SIMS.API.Data;
using SIMS.API.Models;
using SIMS.API.Repositories.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SIMS.API.Repositories.Implementations
{
    public class EnrollmentRepository : IEnrollmentRepository
    {
        private readonly AppDbContext _context;

        public EnrollmentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<CourseStudent> EnrollAsync(CourseStudent enrollment)
        {
            await _context.CourseStudents.AddAsync(enrollment);
            await _context.SaveChangesAsync();
            return enrollment;
        }

        public async Task UnenrollAsync(int courseId, int studentId)
        {
            var enrollment = await GetEnrollmentAsync(courseId, studentId);
            if (enrollment != null)
            {
                _context.CourseStudents.Remove(enrollment);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<CourseStudent?> GetEnrollmentAsync(int courseId, int studentId)
        {
            return await _context.CourseStudents
                .FirstOrDefaultAsync(cs => cs.CourseId == courseId && cs.StudentId == studentId);
        }

        public async Task<IEnumerable<CourseStudent>> GetAllEnrollmentsAsync()
        {
            // Include thông tin liên quan để hiển thị
            return await _context.CourseStudents
                .Include(cs => cs.Course)
                .Include(cs => cs.Student)
                .OrderBy(cs => cs.Course.Name)
                .ThenBy(cs => cs.Student.FullName)
                .ToListAsync();
        }
        public async Task<IEnumerable<CourseStudent>> GetEnrollmentsByStudentIdAsync(int studentId)
        {
            return await _context.CourseStudents
                .Where(cs => cs.StudentId == studentId)
                .Include(cs => cs.Course)
                .ThenInclude(c => c.Teacher) // Lấy cả thông tin giáo viên của môn học
                .OrderBy(cs => cs.Course.Name)
                .ToListAsync();
        }
    }
}
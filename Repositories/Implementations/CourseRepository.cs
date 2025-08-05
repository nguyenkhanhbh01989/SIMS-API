using Microsoft.EntityFrameworkCore;
using SIMS.API.Data;
using SIMS.API.Models;
using SIMS.API.Repositories.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SIMS.API.Repositories.Implementations
{
    public class CourseRepository : ICourseRepository
    {
        private readonly AppDbContext _context;

        public CourseRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Course> CreateAsync(Course course)
        {
            await _context.Courses.AddAsync(course);
            await _context.SaveChangesAsync();
            return course;
        }

        public async Task<IEnumerable<Course>> GetAllAsync()
        {
            // Sử dụng Include để tải thông tin của giáo viên liên quan
            return await _context.Courses
                .Include(c => c.Teacher)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<Course?> GetByIdAsync(int id)
        {
            // Sử dụng Include để tải thông tin của giáo viên liên quan
            return await _context.Courses
                .Include(c => c.Teacher)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Course> UpdateAsync(Course course)
        {
            _context.Courses.Update(course);
            await _context.SaveChangesAsync();
            return course;
        }

        public async Task DeleteAsync(int id)
        {
            var courseToDelete = await _context.Courses.FindAsync(id);
            if (courseToDelete != null)
            {
                _context.Courses.Remove(courseToDelete);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<IEnumerable<Course>> GetByTeacherIdAsync(int teacherId)
        {
            return await _context.Courses
                .Where(c => c.TeacherId == teacherId)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        // SỬA LỖI: Thay thế phương thức GetStudentsByCourseIdAsync bằng phương thức mới này
        public async Task<IEnumerable<CourseStudent>> GetEnrollmentsByCourseIdAsync(int courseId)
        {
            // Truy vấn này sẽ lấy các bản ghi CourseStudent và bao gồm cả thông tin của Student liên quan
            return await _context.CourseStudents
                .Where(cs => cs.CourseId == courseId)
                .Include(cs => cs.Student) // Quan trọng: Lấy thông tin của sinh viên
                .OrderBy(cs => cs.Student.FullName)
                .ToListAsync();
        }
    }
}
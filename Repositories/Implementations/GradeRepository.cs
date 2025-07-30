using Microsoft.EntityFrameworkCore;
using SIMS.API.Data;
using SIMS.API.Models;
using SIMS.API.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SIMS.API.Repositories.Implementations
{
    public class GradeRepository : IGradeRepository
    {
        private readonly AppDbContext _context;

        public GradeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Grade>> GetGradesByCourseIdAsync(int courseId)
        {
            return await _context.Grades
                .Include(g => g.Student)
                .Where(g => g.CourseId == courseId)
                .ToListAsync();
        }

        public async Task<Grade?> GetGradeAsync(int courseId, int studentId)
        {
            return await _context.Grades
                .FirstOrDefaultAsync(g => g.CourseId == courseId && g.StudentId == studentId);
        }

        public async Task<Grade> AddOrUpdateGradeAsync(Grade grade)
        {
            var existingGrade = await GetGradeAsync(grade.CourseId, grade.StudentId);
            if (existingGrade == null)
            {
                // Thêm mới
                await _context.Grades.AddAsync(grade);
            }
            else
            {
                // Cập nhật
                existingGrade.Midterm = grade.Midterm;
                existingGrade.Final = grade.Final;
                existingGrade.Other = grade.Other;
                existingGrade.Total = grade.Total;
                existingGrade.UpdatedAt = grade.UpdatedAt;
                _context.Grades.Update(existingGrade);
            }
            await _context.SaveChangesAsync();
            return grade;
        }
    }
}
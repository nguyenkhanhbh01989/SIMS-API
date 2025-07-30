using SIMS.API.DTOs.Course;
using SIMS.API.DTOs.Teacher;
using SIMS.API.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SIMS.API.Services.Student
{
    public class StudentService : IStudentService
    {
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly IGradeRepository _gradeRepository;

        public StudentService(IEnrollmentRepository enrollmentRepository, IGradeRepository gradeRepository)
        {
            _enrollmentRepository = enrollmentRepository;
            _gradeRepository = gradeRepository;
        }

        public async Task<IEnumerable<CourseViewDto>> GetMyCoursesAsync(int studentId)
        {
            var enrollments = await _enrollmentRepository.GetEnrollmentsByStudentIdAsync(studentId);
            return enrollments.Select(e => new CourseViewDto
            {
                Id = e.Course.Id,
                Name = e.Course.Name,
                Semester = e.Course.Semester,
                TeacherId = e.Course.TeacherId,
                TeacherName = e.Course.Teacher?.FullName,
                CreatedAt = e.Course.CreatedAt
            });
        }

        public async Task<GradeViewDto?> GetMyGradeForCourseAsync(int studentId, int courseId)
        {
            // 1. Xác thực sinh viên có đăng ký môn học này không
            var enrollment = await _enrollmentRepository.GetEnrollmentAsync(courseId, studentId);
            if (enrollment == null)
            {
                throw new ApplicationException("Bạn chưa đăng ký môn học này.");
            }

            // 2. Lấy điểm của sinh viên
            var grade = await _gradeRepository.GetGradeAsync(courseId, studentId);
            if (grade == null)
            {
                return null; // Sinh viên chưa có điểm
            }

            // 3. Map sang DTO để trả về
            return new GradeViewDto
            {
                StudentId = grade.StudentId,
                StudentName = enrollment.Student.FullName ?? "N/A", // Lấy tên từ bản ghi enrollment
                Midterm = grade.Midterm,
                Final = grade.Final,
                Other = grade.Other,
                Total = grade.Total,
                UpdatedAt = grade.UpdatedAt
            };
        }
    }
}
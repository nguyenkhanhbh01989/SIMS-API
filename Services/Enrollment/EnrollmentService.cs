using SIMS.API.DTOs.Enrollment;
using SIMS.API.DTOs.Teacher;
using SIMS.API.Models; // Đảm bảo có using này
using SIMS.API.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SIMS.API.Services.Enrollment
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICourseRepository _courseRepository;

        public EnrollmentService(IEnrollmentRepository enrollmentRepository, IUserRepository userRepository, ICourseRepository courseRepository)
        {
            _enrollmentRepository = enrollmentRepository;
            _userRepository = userRepository;
            _courseRepository = courseRepository;
        }

        public async Task<EnrollmentViewDto> EnrollStudentAsync(EnrollmentRequestDto requestDto)
        {
            var student = await _userRepository.GetUserByIdAsync(requestDto.StudentId);
            if (student == null || student.Role != "Student")
            {
                throw new ApplicationException("ID sinh viên không hợp lệ hoặc người dùng không phải là sinh viên.");
            }

            var course = await _courseRepository.GetByIdAsync(requestDto.CourseId);
            if (course == null)
            {
                throw new ApplicationException("Môn học không tồn tại.");
            }

            var existingEnrollment = await _enrollmentRepository.GetEnrollmentAsync(requestDto.CourseId, requestDto.StudentId);
            if (existingEnrollment != null)
            {
                throw new ApplicationException("Sinh viên đã được đăng ký vào môn học này.");
            }

            var newEnrollment = new CourseStudent
            {
                CourseId = requestDto.CourseId,
                StudentId = requestDto.StudentId,
                EnrolledAt = DateTime.UtcNow
            };

            await _enrollmentRepository.EnrollAsync(newEnrollment);

            // Lỗi xảy ra ở đây, cần sửa lại lời gọi
            return MapToViewDto(student, course, newEnrollment);
        }

        public async Task<bool> UnenrollStudentAsync(int courseId, int studentId)
        {
            var enrollment = await _enrollmentRepository.GetEnrollmentAsync(courseId, studentId);
            if (enrollment == null)
            {
                return false;
            }
            await _enrollmentRepository.UnenrollAsync(courseId, studentId);
            return true;
        }

        public async Task<IEnumerable<EnrollmentViewDto>> GetAllEnrollmentsAsync()
        {
            var enrollments = await _enrollmentRepository.GetAllEnrollmentsAsync();
            // Lỗi xảy ra ở đây, cần sửa lại lời gọi
            return enrollments.Select(e => MapToViewDto(e.Student, e.Course, e));
        }

        public async Task<IEnumerable<StudentInCourseDto>> GetStudentsInCourseAsync(int courseId)
        {
            var students = await _courseRepository.GetStudentsByCourseIdAsync(courseId);
            return students.Select(s => new StudentInCourseDto
            {
                StudentId = s.Id,
                FullName = s.FullName ?? "N/A",
                Email = s.Email,
                EnrolledAt = s.CourseStudents.FirstOrDefault(cs => cs.CourseId == courseId)?.EnrolledAt ?? default
            });
        }

        // SỬA LỖI: Chỉ định rõ ràng kiểu dữ liệu là Models.User và Models.Course
        private EnrollmentViewDto MapToViewDto(Models.User student, Models.Course course, CourseStudent enrollment)
        {
            return new EnrollmentViewDto
            {
                CourseId = course.Id,
                CourseName = course.Name,
                StudentId = student.Id,
                StudentName = student.FullName ?? "N/A",
                StudentEmail = student.Email,
                EnrolledAt = enrollment.EnrolledAt
            };
        }
    }
}
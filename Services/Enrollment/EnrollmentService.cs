using SIMS.API.DTOs.Enrollment;
using SIMS.API.DTOs.Teacher;
using SIMS.API.Models;
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
        private readonly IGradeRepository _gradeRepository; // THÊM DEPENDENCY MỚI

        public EnrollmentService(
            IEnrollmentRepository enrollmentRepository,
            IUserRepository userRepository,
            ICourseRepository courseRepository,
            IGradeRepository gradeRepository) // INJECT VÀO CONSTRUCTOR
        {
            _enrollmentRepository = enrollmentRepository;
            _userRepository = userRepository;
            _courseRepository = courseRepository;
            _gradeRepository = gradeRepository; // GÁN GIÁ TRỊ
        }

        public async Task<EnrollmentViewDto> EnrollStudentAsync(EnrollmentRequestDto requestDto)
        {
            // 1. Kiểm tra sinh viên và môn học (logic không đổi)
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

            // 2. Tạo bản ghi đăng ký mới
            var newEnrollment = new CourseStudent
            {
                CourseId = requestDto.CourseId,
                StudentId = requestDto.StudentId,
                EnrolledAt = DateTime.UtcNow
            };
            await _enrollmentRepository.EnrollAsync(newEnrollment);

            // 3. *** LOGIC MỚI: Tự động tạo một bản ghi điểm trống ***
            var newGradeRecord = new Grade
            {
                CourseId = requestDto.CourseId,
                StudentId = requestDto.StudentId,
                Midterm = null, // Bắt đầu với giá trị NULL
                Final = null,
                Other = null,
                Total = null,
                UpdatedAt = DateTime.UtcNow
            };
            await _gradeRepository.AddOrUpdateGradeAsync(newGradeRecord);

            // 4. Trả về thông tin ghi danh (logic không đổi)
            return MapToViewDto(student, course, newEnrollment);
        }

        // ... (Các phương thức còn lại không thay đổi)
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
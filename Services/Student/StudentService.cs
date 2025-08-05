using SIMS.API.DTOs.Course;
using SIMS.API.DTOs.Teacher;
using SIMS.API.DTOs.Attendance; 
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
        private readonly IAttendanceRepository _attendanceRepository;

        // SỬA LỖI: Cập nhật constructor để nhận IAttendanceRepository
        public StudentService(
            IEnrollmentRepository enrollmentRepository,
            IGradeRepository gradeRepository,
            IAttendanceRepository attendanceRepository)
        {
            _enrollmentRepository = enrollmentRepository;
            _gradeRepository = gradeRepository;
            _attendanceRepository = attendanceRepository; // Gán giá trị
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
            var enrollment = await _enrollmentRepository.GetEnrollmentAsync(courseId, studentId);
            if (enrollment == null)
            {
                throw new ApplicationException("Bạn chưa đăng ký môn học này.");
            }

            var grade = await _gradeRepository.GetGradeAsync(courseId, studentId);
            if (grade == null)
            {
                return null;
            }

            return new GradeViewDto
            {
                StudentId = grade.StudentId,
                StudentName = enrollment.Student.FullName ?? "N/A",
                Midterm = grade.Midterm,
                Final = grade.Final,
                Other = grade.Other,
                Total = grade.Total,
                UpdatedAt = grade.UpdatedAt
            };
        }

        // SỬA LỖI: Triển khai đầy đủ phương thức mới
        public async Task<IEnumerable<AttendanceRecordDto>> GetMyAttendanceForCourseAsync(int studentId, int courseId)
        {
            // 1. Xác thực sinh viên có đăng ký môn học này không
            var enrollment = await _enrollmentRepository.GetEnrollmentAsync(courseId, studentId);
            if (enrollment == null)
            {
                throw new ApplicationException("Bạn chưa đăng ký môn học này.");
            }

            // 2. Lấy lịch sử điểm danh bằng _attendanceRepository
            var attendanceRecords = await _attendanceRepository.GetAttendanceForStudentAsync(courseId, studentId);

            // 3. Map sang DTO để trả về
            return attendanceRecords.Select(a => new AttendanceRecordDto
            {
                StudentId = a.StudentId,
                StudentName = enrollment.Student.FullName ?? "N/A",
                AttendanceDate = a.AttendanceDate,
                IsPresent = a.IsPresent,
                Note = a.Note
            });
        }
    }
}
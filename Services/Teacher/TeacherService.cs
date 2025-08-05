using SIMS.API.DTOs.Course;
using SIMS.API.DTOs.Teacher;
using SIMS.API.Models;
using SIMS.API.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SIMS.API.DTOs.Attendance;
using System.Security.Claims;

namespace SIMS.API.Services.Teacher
{
    public class TeacherService : ITeacherService
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IGradeRepository _gradeRepository;
        private readonly IAttendanceRepository _attendanceRepository;

        public TeacherService(ICourseRepository courseRepository, IGradeRepository gradeRepository, IAttendanceRepository attendanceRepository)
        {
            _courseRepository = courseRepository;
            _gradeRepository = gradeRepository;
            _attendanceRepository = attendanceRepository;
        }

        // Phương thức này vẫn giữ nguyên, chỉ dành cho Teacher
        public async Task<IEnumerable<CourseViewDto>> GetAssignedCoursesAsync(int teacherId)
        {
            var courses = await _courseRepository.GetByTeacherIdAsync(teacherId);
            return courses.Select(c => new CourseViewDto
            {
                Id = c.Id,
                Name = c.Name,
                Semester = c.Semester,
                TeacherId = c.TeacherId,
                TeacherName = c.Teacher?.FullName,
                CreatedAt = c.CreatedAt
            });
        }

        public async Task<IEnumerable<StudentInCourseDto>> GetStudentsInCourseAsync(ClaimsPrincipal user, int courseId)
        {
            await VerifyAccessAsync(user, courseId);

            var students = await _courseRepository.GetStudentsByCourseIdAsync(courseId);
            return students.Select(s => new StudentInCourseDto
            {
                StudentId = s.Id,
                FullName = s.FullName ?? "N/A",
                Email = s.Email,
                EnrolledAt = s.CourseStudents.FirstOrDefault(cs => cs.CourseId == courseId)?.EnrolledAt ?? default
            });
        }

        public async Task<IEnumerable<GradeViewDto>> GetGradesForCourseAsync(ClaimsPrincipal user, int courseId)
        {
            await VerifyAccessAsync(user, courseId);

            var grades = await _gradeRepository.GetGradesByCourseIdAsync(courseId);
            return grades.Select(g => new GradeViewDto
            {
                StudentId = g.StudentId,
                StudentName = g.Student.FullName ?? "N/A",
                Midterm = g.Midterm,
                Final = g.Final,
                Other = g.Other,
                Total = g.Total,
                UpdatedAt = g.UpdatedAt
            });
        }

        // --- PHƯƠNG THỨC ĐÃ ĐƯỢC CẬP NHẬT LOGIC TÍNH ĐIỂM ---
        public async Task<GradeViewDto> UpdateStudentGradeAsync(ClaimsPrincipal user, int courseId, int studentId, UpdateGradeDto gradeDto)
        {
            await VerifyAccessAsync(user, courseId);

            // BƯỚC 1: Tự động tính toán điểm tổng kết từ các điểm thành phần
            var calculatedTotal = CalculateTotalGrade(gradeDto.Midterm, gradeDto.Final, gradeDto.Other);

            // BƯỚC 2: Tạo đối tượng Grade để lưu vào DB.
            // Giá trị Total sẽ LUÔN LÀ giá trị đã được tính toán, bỏ qua giá trị từ DTO.
            var grade = new Grade
            {
                CourseId = courseId,
                StudentId = studentId,
                Midterm = gradeDto.Midterm,
                Final = gradeDto.Final,
                Other = gradeDto.Other,
                Total = calculatedTotal, // Gán giá trị đã được hệ thống tính toán
                UpdatedAt = DateTime.UtcNow
            };

            // BƯỚC 3: Lưu vào DB
            var updatedGrade = await _gradeRepository.AddOrUpdateGradeAsync(grade);
            var student = (await _courseRepository.GetStudentsByCourseIdAsync(courseId)).FirstOrDefault(s => s.Id == studentId);

            // BƯỚC 4: Trả về DTO với dữ liệu chính xác từ DB
            return new GradeViewDto
            {
                StudentId = updatedGrade.StudentId,
                StudentName = student?.FullName ?? "N/A",
                Midterm = updatedGrade.Midterm,
                Final = updatedGrade.Final,
                Other = updatedGrade.Other,
                Total = updatedGrade.Total, // Trả về điểm tổng kết chính xác đã được lưu
                UpdatedAt = updatedGrade.UpdatedAt
            };
        }

        public async Task TakeOrUpdateAttendanceAsync(ClaimsPrincipal user, int courseId, TakeAttendanceRequestDto attendanceDto)
        {
            await VerifyAccessAsync(user, courseId);

            if (attendanceDto.AttendanceDate > DateOnly.FromDateTime(DateTime.UtcNow))
            {
                throw new ApplicationException("Không thể điểm danh cho một ngày trong tương lai.");
            }

            var enrolledStudentIds = (await _courseRepository.GetStudentsByCourseIdAsync(courseId))
                                     .Select(s => s.Id)
                                     .ToHashSet();

            var recordsToUpsert = new List<Attendance>();
            foreach (var studentAttendance in attendanceDto.Attendances)
            {
                if (!enrolledStudentIds.Contains(studentAttendance.StudentId))
                {
                    throw new ApplicationException($"Sinh viên với ID {studentAttendance.StudentId} không có trong môn học này.");
                }

                recordsToUpsert.Add(new Attendance
                {
                    CourseId = courseId,
                    StudentId = studentAttendance.StudentId,
                    AttendanceDate = attendanceDto.AttendanceDate,
                    IsPresent = studentAttendance.IsPresent,
                    Note = studentAttendance.Note
                });
            }

            await _attendanceRepository.UpsertAttendanceAsync(recordsToUpsert);
        }

        public async Task<IEnumerable<AttendanceRecordDto>> GetAttendanceForCourseAsync(ClaimsPrincipal user, int courseId, DateOnly? date)
        {
            await VerifyAccessAsync(user, courseId);

            var attendanceRecords = await _attendanceRepository.GetAttendanceForCourseAsync(courseId, date);
            return attendanceRecords.Select(a => new AttendanceRecordDto
            {
                StudentId = a.StudentId,
                StudentName = a.Student.FullName ?? "N/A",
                AttendanceDate = a.AttendanceDate,
                IsPresent = a.IsPresent,
                Note = a.Note
            });
        }

        private async Task VerifyAccessAsync(ClaimsPrincipal user, int courseId)
        {
            var course = await _courseRepository.GetByIdAsync(courseId);
            if (course == null)
            {
                throw new ApplicationException("Môn học không tồn tại.");
            }

            if (user.IsInRole("Admin"))
            {
                return;
            }

            if (user.IsInRole("Teacher"))
            {
                var teacherIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
                if (teacherIdClaim == null || !int.TryParse(teacherIdClaim.Value, out var teacherId))
                {
                    throw new ApplicationException("Token không hợp lệ hoặc không chứa ID người dùng.");
                }

                if (course.TeacherId == teacherId)
                {
                    return;
                }
            }

            throw new ApplicationException("Bạn không có quyền truy cập vào tài nguyên này.");
        }

        // --- PHƯƠNG THỨC HELPER MỚI ĐỂ TÍNH ĐIỂM ---
        private double? CalculateTotalGrade(double? midterm, double? final, double? other)
        {
            // Định nghĩa trọng số
            const double midtermWeight = 1.0;
            const double finalWeight = 2.0;
            const double otherWeight = 0.5;
            const double totalWeight = midtermWeight + finalWeight + otherWeight; // = 3.5

            // Tránh chia cho 0 nếu tất cả trọng số là 0
            if (totalWeight == 0) return 0;

            // Sử dụng toán tử ?? để coi các giá trị NULL là 0 khi tính tổng
            var weightedSum = (midterm ?? 0) * midtermWeight +
                              (final ?? 0) * finalWeight +
                              (other ?? 0) * otherWeight;

            // Làm tròn đến 2 chữ số thập phân
            return Math.Round(weightedSum / totalWeight, 2);
        }
    }
}
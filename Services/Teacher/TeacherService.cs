using SIMS.API.DTOs.Course;
using SIMS.API.DTOs.Teacher;
using SIMS.API.Models;
using SIMS.API.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using SIMS.API.DTOs.Attendance; 

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

        // ... (Các phương thức khác không thay đổi)
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

        public async Task<IEnumerable<StudentInCourseDto>> GetStudentsInCourseAsync(int teacherId, int courseId)
        {
            await VerifyCourseOwnership(teacherId, courseId);

            var students = await _courseRepository.GetStudentsByCourseIdAsync(courseId);
            return students.Select(s => new StudentInCourseDto
            {
                StudentId = s.Id,
                FullName = s.FullName ?? "N/A",
                Email = s.Email,
                EnrolledAt = s.CourseStudents.FirstOrDefault(cs => cs.CourseId == courseId)?.EnrolledAt ?? default
            });
        }

        public async Task<IEnumerable<GradeViewDto>> GetGradesForCourseAsync(int teacherId, int courseId)
        {
            await VerifyCourseOwnership(teacherId, courseId);

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

        public async Task<GradeViewDto> UpdateStudentGradeAsync(int teacherId, int courseId, int studentId, UpdateGradeDto gradeDto)
        {
            await VerifyCourseOwnership(teacherId, courseId);

            var grade = new Grade
            {
                CourseId = courseId,
                StudentId = studentId,
                Midterm = gradeDto.Midterm,
                Final = gradeDto.Final,
                Other = gradeDto.Other,
                Total = gradeDto.Total,
                UpdatedAt = DateTime.UtcNow
            };

            var updatedGrade = await _gradeRepository.AddOrUpdateGradeAsync(grade);
            var student = (await _courseRepository.GetStudentsByCourseIdAsync(courseId)).FirstOrDefault(s => s.Id == studentId);

            return new GradeViewDto
            {
                StudentId = updatedGrade.StudentId,
                StudentName = student?.FullName ?? "N/A",
                Midterm = updatedGrade.Midterm,
                Final = updatedGrade.Final,
                Other = updatedGrade.Other,
                Total = updatedGrade.Total,
                UpdatedAt = updatedGrade.UpdatedAt
            };
        }

        public async Task TakeOrUpdateAttendanceAsync(int teacherId, int courseId, TakeAttendanceRequestDto attendanceDto)
        {
            await VerifyCourseOwnership(teacherId, courseId);

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

        public async Task<IEnumerable<AttendanceRecordDto>> GetAttendanceForCourseAsync(int teacherId, int courseId, DateOnly? date)
        {
            await VerifyCourseOwnership(teacherId, courseId);

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

        private async Task VerifyCourseOwnership(int teacherId, int courseId)
        {
            var course = await _courseRepository.GetByIdAsync(courseId);
            if (course == null || course.TeacherId != teacherId)
            {
                throw new ApplicationException("Bạn không có quyền truy cập vào môn học này.");
            }
        }
    }
}
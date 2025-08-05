using SIMS.API.DTOs.Attendance;
using SIMS.API.DTOs.Course;
using SIMS.API.DTOs.Teacher;
using System;
using System.Collections.Generic;
using System.Security.Claims; 
using System.Threading.Tasks;

namespace SIMS.API.Services.Teacher
{
    public interface ITeacherService
    {
        // Phương thức này chỉ dành cho Teacher
        Task<IEnumerable<CourseViewDto>> GetAssignedCoursesAsync(int teacherId);

        // SỬA LỖI: Thay đổi các phương thức để nhận ClaimsPrincipal
        Task<IEnumerable<StudentInCourseDto>> GetStudentsInCourseAsync(ClaimsPrincipal user, int courseId);
        Task<IEnumerable<GradeViewDto>> GetGradesForCourseAsync(ClaimsPrincipal user, int courseId);
        Task<GradeViewDto> UpdateStudentGradeAsync(ClaimsPrincipal user, int courseId, int studentId, UpdateGradeDto gradeDto);
        Task TakeOrUpdateAttendanceAsync(ClaimsPrincipal user, int courseId, TakeAttendanceRequestDto attendanceDto);
        Task<IEnumerable<AttendanceRecordDto>> GetAttendanceForCourseAsync(ClaimsPrincipal user, int courseId, DateOnly? date);
    }
}
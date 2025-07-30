using SIMS.API.DTOs.Course;
using SIMS.API.DTOs.Teacher;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SIMS.API.DTOs.Attendance; // ĐÂY LÀ DÒNG CẦN THIẾT

namespace SIMS.API.Services.Teacher
{
    public interface ITeacherService
    {
        Task<IEnumerable<CourseViewDto>> GetAssignedCoursesAsync(int teacherId);
        Task<IEnumerable<StudentInCourseDto>> GetStudentsInCourseAsync(int teacherId, int courseId);
        Task<IEnumerable<GradeViewDto>> GetGradesForCourseAsync(int teacherId, int courseId);
        Task<GradeViewDto> UpdateStudentGradeAsync(int teacherId, int courseId, int studentId, UpdateGradeDto gradeDto);

        // --- PHƯƠNG THỨC MỚI CHO ĐIỂM DANH ---
        Task TakeOrUpdateAttendanceAsync(int teacherId, int courseId, TakeAttendanceRequestDto attendanceDto);
        Task<IEnumerable<AttendanceRecordDto>> GetAttendanceForCourseAsync(int teacherId, int courseId, DateOnly? date);
    }
}
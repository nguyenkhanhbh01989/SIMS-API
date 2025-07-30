using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIMS.API.DTOs.Teacher;
using SIMS.API.Services.Teacher;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using SIMS.API.DTOs.Attendance; 
namespace SIMS.API.Controllers
{
    [Route("api/teacher")]
    [ApiController]
    [Authorize(Roles = "Teacher,Admin")]
    public class TeacherController : ControllerBase
    {
        private readonly ITeacherService _teacherService;

        public TeacherController(ITeacherService teacherService)
        {
            _teacherService = teacherService;
        }

        // ... (Các endpoint khác không thay đổi)
        [HttpGet("courses")]
        public async Task<IActionResult> GetMyCourses()
        {
            var teacherId = GetCurrentUserId();
            var courses = await _teacherService.GetAssignedCoursesAsync(teacherId);
            return Ok(courses);
        }

        [HttpGet("courses/{courseId}/students")]
        public async Task<IActionResult> GetStudentsInCourse(int courseId)
        {
            try
            {
                var teacherId = GetCurrentUserId();
                var students = await _teacherService.GetStudentsInCourseAsync(teacherId, courseId);
                return Ok(students);
            }
            catch (ApplicationException ex)
            {
                return Forbid(ex.Message);
            }
        }

        [HttpGet("courses/{courseId}/grades")]
        public async Task<IActionResult> GetGrades(int courseId)
        {
            try
            {
                var teacherId = GetCurrentUserId();
                var grades = await _teacherService.GetGradesForCourseAsync(teacherId, courseId);
                return Ok(grades);
            }
            catch (ApplicationException ex)
            {
                return Forbid(ex.Message);
            }
        }

        [HttpPut("courses/{courseId}/grades/{studentId}")]
        public async Task<IActionResult> UpdateGrade(int courseId, int studentId, [FromBody] UpdateGradeDto gradeDto)
        {
            try
            {
                var teacherId = GetCurrentUserId();
                var updatedGrade = await _teacherService.UpdateStudentGradeAsync(teacherId, courseId, studentId, gradeDto);
                return Ok(updatedGrade);
            }
            catch (ApplicationException ex)
            {
                return Forbid(ex.Message);
            }
        }

        // --- ENDPOINTS MỚI CHO ĐIỂM DANH ---

        [HttpPost("courses/{courseId}/attendance")]
        public async Task<IActionResult> TakeAttendance(int courseId, [FromBody] TakeAttendanceRequestDto attendanceDto)
        {
            try
            {
                var teacherId = GetCurrentUserId();
                await _teacherService.TakeOrUpdateAttendanceAsync(teacherId, courseId, attendanceDto);
                return Ok(new { message = $"Đã ghi nhận điểm danh cho ngày {attendanceDto.AttendanceDate}." });
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("courses/{courseId}/attendance")]
        public async Task<IActionResult> GetAttendance(int courseId, [FromQuery] DateOnly? date)
        {
            try
            {
                var teacherId = GetCurrentUserId();
                var records = await _teacherService.GetAttendanceForCourseAsync(teacherId, courseId, date);
                return Ok(records);
            }
            catch (ApplicationException ex)
            {
                return Forbid(ex.Message);
            }
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
            {
                throw new InvalidOperationException("User ID not found in token.");
            }
            return userId;
        }
    }
}
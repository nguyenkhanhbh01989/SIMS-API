using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIMS.API.DTOs.Teacher;
using SIMS.API.Services.Teacher;
using SIMS.API.Services.Schedule; // Thêm using này
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
        private readonly IScheduleService _scheduleService; // SỬA LỖI: Khai báo trường

        // SỬA LỖI: Cập nhật constructor để nhận cả hai service
        public TeacherController(ITeacherService teacherService, IScheduleService scheduleService)
        {
            _teacherService = teacherService;
            _scheduleService = scheduleService; // Gán giá trị
        }

        [HttpGet("courses")]
        public async Task<IActionResult> GetMyCourses()
        {
            if (!User.IsInRole("Teacher"))
            {
                return Forbid("Chỉ giáo viên mới có thể xem danh sách lớp học của mình.");
            }
            var teacherId = GetCurrentUserId();
            var courses = await _teacherService.GetAssignedCoursesAsync(teacherId);
            return Ok(courses);
        }

        [HttpGet("courses/{courseId}/students")]
        public async Task<IActionResult> GetStudentsInCourse(int courseId)
        {
            try
            {
                var students = await _teacherService.GetStudentsInCourseAsync(User, courseId);
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
                var grades = await _teacherService.GetGradesForCourseAsync(User, courseId);
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
                var updatedGrade = await _teacherService.UpdateStudentGradeAsync(User, courseId, studentId, gradeDto);
                return Ok(updatedGrade);
            }
            catch (ApplicationException ex)
            {
                return Forbid(ex.Message);
            }
        }

        [HttpPost("courses/{courseId}/attendance")]
        public async Task<IActionResult> TakeAttendance(int courseId, [FromBody] TakeAttendanceRequestDto attendanceDto)
        {
            try
            {
                await _teacherService.TakeOrUpdateAttendanceAsync(User, courseId, attendanceDto);
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
                var records = await _teacherService.GetAttendanceForCourseAsync(User, courseId, date);
                return Ok(records);
            }
            catch (ApplicationException ex)
            {
                return Forbid(ex.Message);
            }
        }

        [HttpGet("my-schedule")]
        public async Task<IActionResult> GetMySchedule()
        {
            if (!User.IsInRole("Teacher")) return Forbid();
            var teacherId = GetCurrentUserId();
            var scheduleEvents = await _scheduleService.GetTeacherScheduleAsync(teacherId); 
            return Ok(scheduleEvents);
        }

        // SỬA LỖI: Đảm bảo phương thức này nằm BÊN TRONG class
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
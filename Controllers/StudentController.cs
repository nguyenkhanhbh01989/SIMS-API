using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIMS.API.Services.Student;
using SIMS.API.Services.Schedule; // Thêm using này
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SIMS.API.Controllers
{
    [Route("api/student")]
    [ApiController]
    [Authorize(Roles = "Student")]
    public class StudentController : ControllerBase
    {
        private readonly IStudentService _studentService;
        private readonly IScheduleService _scheduleService; // SỬA LỖI: Khai báo trường

        // SỬA LỖI: Cập nhật constructor để nhận cả hai service
        public StudentController(IStudentService studentService, IScheduleService scheduleService)
        {
            _studentService = studentService;
            _scheduleService = scheduleService; // Gán giá trị
        }

        // GET /api/student/my-courses
        [HttpGet("my-courses")]
        public async Task<IActionResult> GetMyCourses()
        {
            var studentId = GetCurrentUserId();
            var courses = await _studentService.GetMyCoursesAsync(studentId);
            return Ok(courses);
        }

        // GET /api/student/my-grades/{courseId}
        [HttpGet("my-grades/{courseId}")]
        public async Task<IActionResult> GetMyGradeForCourse(int courseId)
        {
            try
            {
                var studentId = GetCurrentUserId();
                var grade = await _studentService.GetMyGradeForCourseAsync(studentId, courseId);
                if (grade == null)
                {
                    return NotFound(new { message = "Bạn chưa có điểm cho môn học này." });
                }
                return Ok(grade);
            }
            catch (ApplicationException ex)
            {
                return Forbid(ex.Message);
            }
        }

        // GET /api/student/my-attendance/{courseId}
        [HttpGet("my-attendance/{courseId}")]
        public async Task<IActionResult> GetMyAttendanceForCourse(int courseId)
        {
            try
            {
                var studentId = GetCurrentUserId();
                var records = await _studentService.GetMyAttendanceForCourseAsync(studentId, courseId);
                return Ok(records);
            }
            catch (ApplicationException ex)
            {
                return Forbid(ex.Message);
            }
        }

        // GET /api/student/my-schedule
        [HttpGet("my-schedule")]
        public async Task<IActionResult> GetMySchedule()
        {
            var studentId = GetCurrentUserId();
            var schedule = await _scheduleService.GetStudentScheduleAsync(studentId);
            return Ok(schedule);
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
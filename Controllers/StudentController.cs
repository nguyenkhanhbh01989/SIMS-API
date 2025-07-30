using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIMS.API.Services.Student;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SIMS.API.Controllers
{
    [Route("api/student")]
    [ApiController]
    [Authorize(Roles = "Student")] // CHỈ SINH VIÊN MỚI CÓ QUYỀN
    public class StudentController : ControllerBase
    {
        private readonly IStudentService _studentService;

        public StudentController(IStudentService studentService)
        {
            _studentService = studentService;
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
                return Forbid(ex.Message); // 403 Forbidden nếu không đăng ký môn học
            }
        }

        private int GetCurrentUserId()
        {
            // Lấy ID của người dùng đang đăng nhập từ JWT token
            // Đảm bảo an toàn, sinh viên chỉ có thể xem dữ liệu của chính mình
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
            {
                throw new InvalidOperationException("User ID not found in token.");
            }
            return userId;
        }
    }
}
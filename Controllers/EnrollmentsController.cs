using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIMS.API.DTOs.Enrollment;
using SIMS.API.Services.Enrollment;
using System;
using System.Threading.Tasks;

namespace SIMS.API.Controllers
{
    [Route("api/enrollments")]
    [ApiController]
    [Authorize(Roles = "Admin")] // Chỉ Admin có quyền quản lý đăng ký
    public class EnrollmentsController : ControllerBase
    {
        private readonly IEnrollmentService _enrollmentService;

        public EnrollmentsController(IEnrollmentService enrollmentService)
        {
            _enrollmentService = enrollmentService;
        }

        // POST /api/enrollments
        [HttpPost]
        public async Task<IActionResult> EnrollStudent([FromBody] EnrollmentRequestDto requestDto)
        {
            try
            {
                var result = await _enrollmentService.EnrollStudentAsync(requestDto);
                return Ok(result);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET /api/enrollments
        [HttpGet]
        public async Task<IActionResult> GetAllEnrollments()
        {
            var enrollments = await _enrollmentService.GetAllEnrollmentsAsync();
            return Ok(enrollments);
        }

        // GET /api/enrollments/course/{courseId}
        [HttpGet("course/{courseId}")]
        public async Task<IActionResult> GetStudentsInCourse(int courseId)
        {
            var students = await _enrollmentService.GetStudentsInCourseAsync(courseId);
            return Ok(students);
        }

        // DELETE /api/enrollments/course/{courseId}/student/{studentId}
        [HttpDelete("course/{courseId}/student/{studentId}")]
        public async Task<IActionResult> UnenrollStudent(int courseId, int studentId)
        {
            var success = await _enrollmentService.UnenrollStudentAsync(courseId, studentId);
            if (!success)
            {
                return NotFound(new { message = "Không tìm thấy bản ghi đăng ký để hủy." });
            }
            return NoContent(); // 204 No Content khi thành công
        }
    }
}
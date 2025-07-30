using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIMS.API.DTOs.Course;
using SIMS.API.Services.Course;
using System;
using System.Threading.Tasks;

namespace SIMS.API.Controllers
{
    [Route("api/courses")]
    [ApiController]
    [Authorize(Roles = "Admin")] // CHỈ ADMIN MỚI CÓ QUYỀN
    public class CoursesController : ControllerBase
    {
        private readonly ICourseService _courseService;

        public CoursesController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        // POST /api/courses
        [HttpPost]
        public async Task<IActionResult> CreateCourse([FromBody] CreateUpdateCourseDto courseDto)
        {
            try
            {
                var newCourse = await _courseService.CreateCourseAsync(courseDto);
                return CreatedAtAction(nameof(GetCourseById), new { id = newCourse.Id }, newCourse);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET /api/courses
        [HttpGet]
        public async Task<IActionResult> GetAllCourses()
        {
            var courses = await _courseService.GetAllCoursesAsync();
            return Ok(courses);
        }

        // GET /api/courses/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCourseById(int id)
        {
            var course = await _courseService.GetCourseByIdAsync(id);
            if (course == null)
            {
                return NotFound(new { message = "Không tìm thấy môn học." });
            }
            return Ok(course);
        }

        // PUT /api/courses/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCourse(int id, [FromBody] CreateUpdateCourseDto courseDto)
        {
            try
            {
                var updatedCourse = await _courseService.UpdateCourseAsync(id, courseDto);
                return Ok(updatedCourse);
            }
            catch (ApplicationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // DELETE /api/courses/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var success = await _courseService.DeleteCourseAsync(id);
            if (!success)
            {
                return NotFound(new { message = "Không tìm thấy môn học để xóa." });
            }
            return NoContent(); // Trả về 204 No Content khi xóa thành công
        }
    }
}
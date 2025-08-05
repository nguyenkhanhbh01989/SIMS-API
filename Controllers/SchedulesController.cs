using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIMS.API.DTOs.Schedule;
using SIMS.API.Services.Schedule;
using System;
using System.Threading.Tasks;

namespace SIMS.API.Controllers
{
    [Route("api/schedules")]
    [ApiController]
    [Authorize(Roles = "Admin")] // Chỉ Admin có quyền quản lý lịch học
    public class SchedulesController : ControllerBase
    {
        private readonly IScheduleService _scheduleService;

        public SchedulesController(IScheduleService scheduleService)
        {
            _scheduleService = scheduleService;
        }

        // GET /api/schedules
        [HttpGet]
        public async Task<IActionResult> GetAllSchedules()
        {
            var schedules = await _scheduleService.GetAllSchedulesAsync();
            return Ok(schedules);
        }

        // POST /api/schedules
        [HttpPost]
        public async Task<IActionResult> CreateSchedule([FromBody] ScheduleCreateDto scheduleDto)
        {
            try
            {
                var newSchedule = await _scheduleService.CreateScheduleAsync(scheduleDto);
                return Ok(newSchedule);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT /api/schedules/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSchedule(int id, [FromBody] ScheduleUpdateDto scheduleDto)
        {
            try
            {
                var updatedSchedule = await _scheduleService.UpdateScheduleAsync(id, scheduleDto);
                return Ok(updatedSchedule);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE /api/schedules/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSchedule(int id)
        {
            var success = await _scheduleService.DeleteScheduleAsync(id);
            if (!success)
            {
                return NotFound(new { message = "Không tìm thấy lịch học để xóa." });
            }
            return NoContent();
        }
    }
}
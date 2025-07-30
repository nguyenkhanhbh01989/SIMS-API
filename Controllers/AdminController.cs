using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIMS.API.DTOs.Admin;
using SIMS.API.DTOs.User;
using SIMS.API.Services.User;
using System;
using System.Threading.Tasks;

namespace SIMS.API.Controllers
{
    [Route("api/admin/users")]
    [ApiController]
    [Authorize(Roles = "Admin")] // CHỈ ADMIN MỚI CÓ QUYỀN TRUY CẬP CONTROLLER NÀY
    public class AdminController : ControllerBase
    {
        private readonly IUserService _userService;

        public AdminController(IUserService userService)
        {
            _userService = userService;
        }

        // POST /api/admin/users
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] AdminCreateUserDto createUserDto)
        {
            try
            {
                var createdUser = await _userService.CreateUserByAdminAsync(createUserDto);
                // Trả về 201 Created với thông tin người dùng vừa tạo
                return CreatedAtAction(nameof(GetUserById), new { userId = createdUser.Id }, createdUser);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET /api/admin/users
        // GET /api/admin/users?role=Student
        [HttpGet]
        public async Task<IActionResult> GetAllUsers([FromQuery] string? role)
        {
            var users = await _userService.GetAllUsersByAdminAsync(role);
            return Ok(users);
        }

        // GET /api/admin/users/{userId}
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserById(int userId)
        {
            var user = await _userService.GetUserByIdByAdminAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "Không tìm thấy người dùng." });
            }
            return Ok(user);
        }

        // PUT /api/admin/users/{userId}
        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateUser(int userId, [FromBody] UserProfileUpdateDto profileDto)
        {
            try
            {
                await _userService.UpdateUserByAdminAsync(userId, profileDto);
                return Ok(new { message = "Cập nhật thông tin người dùng thành công." });
            }
            catch (ApplicationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // PATCH /api/admin/users/{userId}/toggle-status
        [HttpPatch("{userId}/toggle-status")]
        public async Task<IActionResult> ToggleUserStatus(int userId)
        {
            try
            {
                var newStatus = await _userService.ToggleUserStatusAsync(userId);
                var statusMessage = newStatus ? "hoạt động" : "vô hiệu hóa";
                return Ok(new { message = $"Tài khoản đã được {statusMessage}." });
            }
            catch (ApplicationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
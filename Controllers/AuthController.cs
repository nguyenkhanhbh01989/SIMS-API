using Microsoft.AspNetCore.Mvc;
using SIMS.API.DTOs.Auth;
using SIMS.API.Services.Auth; // SỬA LỖI: Khai báo using đúng cho IAuthService

namespace SIMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequest)
        {
            try
            {
                // SỬA LỖI: Gọi đúng tên phương thức LoginAsync đã được định nghĩa trong IAuthService
                var response = await _authService.LoginAsync(loginRequest);
                return Ok(response);
            }
            catch (ApplicationException ex)
            {
                // Lỗi nghiệp vụ có chủ đích (ví dụ: token không hợp lệ, tài khoản bị khóa)
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // Lỗi hệ thống không mong muốn
                _logger.LogError(ex, "An unexpected error occurred during the login process.");
                return StatusCode(500, new { message = "Đã có lỗi không mong muốn xảy ra trong hệ thống." });
            }
        }
    }
}
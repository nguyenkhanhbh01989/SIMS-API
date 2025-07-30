using SIMS.API.DTOs.User;

namespace SIMS.API.DTOs.Auth
{
    public class LoginResponseDto
    {
        public required string JwtToken { get; set; }
        public bool IsProfileIncomplete { get; set; }
        public required UserDto User { get; set; }
    }
}
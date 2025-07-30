using SIMS.API.DTOs.Auth;

namespace SIMS.API.Services.Auth
{
    public interface IAuthService
    {
        Task<LoginResponseDto> LoginAsync(LoginRequestDto loginRequest);
    }
}
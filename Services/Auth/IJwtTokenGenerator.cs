using SIMS.API.Models;

namespace SIMS.API.Services.Auth
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(Models.User user);
    }
}
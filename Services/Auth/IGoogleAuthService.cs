namespace SIMS.API.Services.Auth
{
    public interface IGoogleAuthService
    {
        Task<string?> ValidateGoogleTokenAndGetEmailAsync(string idToken);
    }
}
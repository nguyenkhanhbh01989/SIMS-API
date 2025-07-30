using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace SIMS.API.Services.Auth
{
    public class GoogleAuthService : IGoogleAuthService
    {
        private readonly ILogger<GoogleAuthService> _logger;
        private readonly string _googleClientId;

        public GoogleAuthService(IConfiguration configuration, ILogger<GoogleAuthService> logger)
        {
            _logger = logger;
            _googleClientId = configuration["Authentication:Google:ClientId"]
                              ?? throw new InvalidOperationException("Google Client ID is not configured in appsettings.json.");
        }

        public async Task<string?> ValidateGoogleTokenAndGetEmailAsync(string idToken)
        {
            if (string.IsNullOrEmpty(idToken))
            {
                return null;
            }

            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings()
                {
                    Audience = new[] { _googleClientId }
                };

                var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);

                if (payload.EmailVerified)
                {
                    return payload.Email;
                }

                _logger.LogWarning("Google token email not verified for {Email}", payload.Email);
                return null;
            }
            catch (InvalidJwtException ex)
            {
                _logger.LogError(ex, "Invalid Google ID Token provided.");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred during Google token validation.");
                return null;
            }
        }
    }
}
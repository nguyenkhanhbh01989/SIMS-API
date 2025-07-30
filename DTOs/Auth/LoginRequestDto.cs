using System.ComponentModel.DataAnnotations;

namespace SIMS.API.DTOs.Auth
{
    public class LoginRequestDto
    {
        [Required]
        public required string GoogleIdToken { get; set; }
    }
}
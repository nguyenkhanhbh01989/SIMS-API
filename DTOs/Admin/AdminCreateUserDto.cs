using System.ComponentModel.DataAnnotations;

namespace SIMS.API.DTOs.Admin
{
    public class AdminCreateUserDto
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        // Có thể thêm validation để đảm bảo Role chỉ là 'Student' hoặc 'Teacher'
        public required string Role { get; set; }
    }
}
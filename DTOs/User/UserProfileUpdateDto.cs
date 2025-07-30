using System.ComponentModel.DataAnnotations;

namespace SIMS.API.DTOs.User
{
    public class UserProfileUpdateDto
    {
        [Required]
        [StringLength(100)]
        public required string FullName { get; set; }

        public bool? Gender { get; set; }

        [StringLength(20)]
        public string? PhoneNumber { get; set; }

        public DateOnly? DateOfBirth { get; set; }

        [StringLength(100)]
        public string? Department { get; set; }
    }
}
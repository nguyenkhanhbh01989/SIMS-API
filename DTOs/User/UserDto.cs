﻿namespace SIMS.API.DTOs.User
{
    public class UserDto
    {
        public int Id { get; set; }
        public required string Email { get; set; }
        public required string Role { get; set; }
        public string? FullName { get; set; }
    }
}
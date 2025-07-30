using SIMS.API.DTOs.Admin; // Thêm using mới
using SIMS.API.DTOs.User;
using System.Collections.Generic; // Thêm using mới
using System.Threading.Tasks; // Thêm using mới

namespace SIMS.API.Services.User
{
    public interface IUserService
    {
        // Chức năng cho người dùng cá nhân
        Task UpdateProfileAsync(int userId, UserProfileUpdateDto profileDto);

        // --- CHỨC NĂNG MỚI CHO ADMIN ---
        Task<AdminUserViewDto> CreateUserByAdminAsync(AdminCreateUserDto createUserDto);
        Task<IEnumerable<AdminUserViewDto>> GetAllUsersByAdminAsync(string? role);
        Task<AdminUserViewDto?> GetUserByIdByAdminAsync(int userId);
        Task UpdateUserByAdminAsync(int userId, UserProfileUpdateDto profileDto);
        Task<bool> ToggleUserStatusAsync(int userId);
    }
}
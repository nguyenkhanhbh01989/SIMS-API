using SIMS.API.DTOs.Admin;
using SIMS.API.DTOs.User;
using SIMS.API.Models;
using SIMS.API.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SIMS.API.Services.User
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // --- Chức năng cho người dùng cá nhân (không đổi) ---
        public async Task UpdateProfileAsync(int userId, UserProfileUpdateDto profileDto)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null) throw new ApplicationException("Không tìm thấy người dùng.");

            user.FullName = profileDto.FullName;
            user.Gender = profileDto.Gender;
            user.PhoneNumber = profileDto.PhoneNumber;
            user.DateOfBirth = profileDto.DateOfBirth;
            user.Department = profileDto.Department;

            await _userRepository.UpdateUserAsync(user);
        }

        // --- TRIỂN KHAI CHỨC NĂNG MỚI CHO ADMIN ---

        public async Task<AdminUserViewDto> CreateUserByAdminAsync(AdminCreateUserDto createUserDto)
        {
            if (await _userRepository.UserExistsByEmailAsync(createUserDto.Email))
            {
                throw new ApplicationException($"Email '{createUserDto.Email}' đã tồn tại trong hệ thống.");
            }

            var newUser = new Models.User
            {
                Email = createUserDto.Email,
                Role = createUserDto.Role,
                IsActive = true, // Mặc định là hoạt động
                CreatedAt = DateTime.UtcNow
            };

            var createdUser = await _userRepository.CreateUserAsync(newUser);
            return MapUserToAdminViewDto(createdUser);
        }

        public async Task<IEnumerable<AdminUserViewDto>> GetAllUsersByAdminAsync(string? role)
        {
            var users = await _userRepository.GetAllAsync();

            // Lọc theo vai trò nếu có tham số role được cung cấp
            if (!string.IsNullOrEmpty(role))
            {
                users = users.Where(u => u.Role.Equals(role, StringComparison.OrdinalIgnoreCase));
            }

            return users.Select(MapUserToAdminViewDto);
        }

        public async Task<AdminUserViewDto?> GetUserByIdByAdminAsync(int userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            return user == null ? null : MapUserToAdminViewDto(user);
        }

        public async Task UpdateUserByAdminAsync(int userId, UserProfileUpdateDto profileDto)
        {
            // Tái sử dụng logic của UpdateProfileAsync
            await UpdateProfileAsync(userId, profileDto);
        }

        public async Task<bool> ToggleUserStatusAsync(int userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                throw new ApplicationException("Không tìm thấy người dùng để thay đổi trạng thái.");
            }

            // Đảo ngược trạng thái IsActive
            user.IsActive = !user.IsActive;
            await _userRepository.UpdateUserAsync(user);
            return user.IsActive;
        }

        // Phương thức private helper để tránh lặp code mapping
        private AdminUserViewDto MapUserToAdminViewDto(Models.User user)
        {
            return new AdminUserViewDto
            {
                Id = user.Id,
                Email = user.Email,
                Role = user.Role,
                FullName = user.FullName,
                Gender = user.Gender,
                PhoneNumber = user.PhoneNumber,
                DateOfBirth = user.DateOfBirth,
                Department = user.Department,
                CreatedAt = user.CreatedAt,
                IsActive = user.IsActive
            };
        }
    }
}
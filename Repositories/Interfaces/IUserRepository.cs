using SIMS.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SIMS.API.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> GetUserByIdAsync(int id);
        Task UpdateUserAsync(User user);

        // --- PHƯƠNG THỨC MỚI CHO ADMIN ---
        Task<bool> UserExistsByEmailAsync(string email);
        Task<User> CreateUserAsync(User user);
        Task<IEnumerable<User>> GetAllAsync();
    }
}
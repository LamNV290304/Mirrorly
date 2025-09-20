using Mirrorly.Models;

namespace Mirrorly.Services.Interfaces
{
    public interface IAuthServices
    {
        Task<User?> Login(string email, string password);
        Task<bool> Register(User user, string password, byte roleId);
        Task<User?> GetUserByEmail(string email);
        Task<User?> GetUserById(int userId);
        Task<bool> IsEmailExists(string email);
        Task<bool> IsUsernameExists(string username);
        Task<bool> VerifyPassword(string password, byte[] passwordHash);
        byte[] HashPassword(string password);
    }
}

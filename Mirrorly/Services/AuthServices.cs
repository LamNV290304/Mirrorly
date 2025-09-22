using Microsoft.EntityFrameworkCore;
using Mirrorly.Models;
using Mirrorly.Services.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace Mirrorly.Services
{
    public class AuthServices : IAuthServices
    {
        private readonly ProjectExeContext _context;

        public AuthServices(ProjectExeContext context)
        {
            _context = context;
        }

        public async Task<User?> Login(string email, string password)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .Include(u => u.CustomerProfile)
                .Include(u => u.Muaprofile)
                .FirstOrDefaultAsync(u => u.Email == email && u.IsActive);

            if (user != null && VerifyPassword(password, user.PasswordHash).Result)
            {
                return user;
            }
            return null;
        }

        public async Task<bool> Register(User user, string password, byte roleId)
        {
            try
            {
                user.PasswordHash = HashPassword(password);
                user.RoleId = roleId;
                user.IsActive = true;
                user.IsEmailVerified = false;

                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<User?> GetUserByEmail(string email)
        {
            return await _context.Users
                .Include(u => u.Role)
                .Include(u => u.CustomerProfile)
                .Include(u => u.Muaprofile)
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetUserById(int userId)
        {
            return await _context.Users
                .Include(u => u.Role)
                .Include(u => u.CustomerProfile)
                .Include(u => u.Muaprofile)
                .FirstOrDefaultAsync(u => u.UserId == userId);
        }

        public async Task<bool> IsEmailExists(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }

        public async Task<bool> IsUsernameExists(string username)
        {
            return await _context.Users.AnyAsync(u => u.Username == username);
        }

        public async Task<bool> VerifyPassword(string password, byte[] passwordHash)
        {
            var hashedInput = HashPassword(password);
            return hashedInput.SequenceEqual(passwordHash);
        }

        public byte[] HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                return sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        public async Task<bool> ChangePassword(int userId, string oldPassword, string newPassword)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null) return false;

            // kiểm tra mật khẩu cũ
            if (!await VerifyPassword(oldPassword, user.PasswordHash))
                return false;

            // cập nhật mật khẩu mới
            user.PasswordHash = HashPassword(newPassword);
            await _context.SaveChangesAsync();
            return true;
        }

    }
}
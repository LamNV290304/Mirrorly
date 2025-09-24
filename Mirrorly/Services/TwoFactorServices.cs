using Microsoft.EntityFrameworkCore;
using Mirrorly.Models;
using Mirrorly.Services.Interfaces;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Mirrorly.Services
{
    public class TwoFactorServices : ITwoFactorServices
    {
        private readonly ProjectExeContext _context;
        private const int LOCKOUT_DURATION_MINUTES = 15;
        private const int MAX_FAILED_ATTEMPTS = 5;

        public TwoFactorServices(ProjectExeContext context)
        {
            _context = context;
        }

        public async Task<bool> Enable2FAAsync(int userId, string verificationCode)
        {
            try
            {
                // Get or create 2FA record
                var twoFactorAuth = await _context.TwoFactorAuths
                    .FirstOrDefaultAsync(t => t.UserId == userId);

                if (twoFactorAuth == null)
                {
                    // Generate secret key if not exists
                    var secretKey = GenerateSecretKey();
                    twoFactorAuth = new TwoFactorAuth
                    {
                        UserId = userId,
                        SecretKey = secretKey,
                        IsEnabled = false
                    };
                    _context.TwoFactorAuths.Add(twoFactorAuth);
                    await _context.SaveChangesAsync();
                }

                // Verify the code before enabling
                if (!VerifyTOTPCode(twoFactorAuth.SecretKey!, verificationCode))
                {
                    return false;
                }

                // Generate backup codes
                var backupCodes = await GenerateBackupCodesAsync();

                // Enable 2FA
                twoFactorAuth.IsEnabled = true;
                twoFactorAuth.BackupCodes = JsonSerializer.Serialize(backupCodes);
                twoFactorAuth.EnabledAt = DateTime.UtcNow;
                twoFactorAuth.FailedAttempts = 0;
                twoFactorAuth.LockedUntil = null;

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> Disable2FAAsync(int userId)
        {
            try
            {
                var twoFactorAuth = await _context.TwoFactorAuths
                    .FirstOrDefaultAsync(t => t.UserId == userId);

                if (twoFactorAuth == null)
                    return false;

                twoFactorAuth.IsEnabled = false;
                twoFactorAuth.SecretKey = null;
                twoFactorAuth.BackupCodes = null;
                twoFactorAuth.EnabledAt = null;
                twoFactorAuth.FailedAttempts = 0;
                twoFactorAuth.LockedUntil = null;

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> Verify2FACodeAsync(int userId, string code)
        {
            try
            {
                var twoFactorAuth = await _context.TwoFactorAuths
                    .FirstOrDefaultAsync(t => t.UserId == userId && t.IsEnabled);

                if (twoFactorAuth == null)
                    return false;

                // Check if account is locked out
                if (await IsLockedOutAsync(userId))
                    return false;

                // Verify TOTP code
                var isValid = VerifyTOTPCode(twoFactorAuth.SecretKey!, code);

                if (isValid)
                {
                    // Reset failed attempts on successful verification
                    twoFactorAuth.FailedAttempts = 0;
                    twoFactorAuth.LastUsedAt = DateTime.UtcNow;
                    twoFactorAuth.LockedUntil = null;
                    await _context.SaveChangesAsync();
                    return true;
                }
                else
                {
                    // Record failed attempt
                    await RecordFailedAttemptAsync(userId);
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> VerifyBackupCodeAsync(int userId, string backupCode)
        {
            try
            {
                var twoFactorAuth = await _context.TwoFactorAuths
                    .FirstOrDefaultAsync(t => t.UserId == userId && t.IsEnabled);

                if (twoFactorAuth == null || string.IsNullOrEmpty(twoFactorAuth.BackupCodes))
                    return false;

                var backupCodes = JsonSerializer.Deserialize<List<string>>(twoFactorAuth.BackupCodes);
                if (backupCodes == null || !backupCodes.Contains(backupCode))
                    return false;

                // Remove used backup code
                backupCodes.Remove(backupCode);
                twoFactorAuth.BackupCodes = JsonSerializer.Serialize(backupCodes);
                twoFactorAuth.LastUsedAt = DateTime.UtcNow;
                twoFactorAuth.FailedAttempts = 0;
                twoFactorAuth.LockedUntil = null;

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<string> GenerateSecretKeyAsync(int userId)
        {
            var secretKey = GenerateSecretKey();

            var twoFactorAuth = await _context.TwoFactorAuths
                .FirstOrDefaultAsync(t => t.UserId == userId);

            if (twoFactorAuth == null)
            {
                twoFactorAuth = new TwoFactorAuth
                {
                    UserId = userId,
                    SecretKey = secretKey,
                    IsEnabled = false
                };
                _context.TwoFactorAuths.Add(twoFactorAuth);
            }
            else
            {
                twoFactorAuth.SecretKey = secretKey;
            }

            await _context.SaveChangesAsync();
            return secretKey;
        }

        public async Task<List<string>> GenerateBackupCodesAsync()
        {
            var codes = new List<string>();
            for (int i = 0; i < 10; i++)
            {
                codes.Add(GenerateBackupCode());
            }
            return codes;
        }

        public async Task<TwoFactorAuth?> Get2FASettingsAsync(int userId)
        {
            return await _context.TwoFactorAuths
                .FirstOrDefaultAsync(t => t.UserId == userId);
        }

        public async Task<bool> Is2FAEnabledAsync(int userId)
        {
            return await _context.TwoFactorAuths
                .AnyAsync(t => t.UserId == userId && t.IsEnabled);
        }

        public async Task<bool> RecordFailedAttemptAsync(int userId)
        {
            try
            {
                var twoFactorAuth = await _context.TwoFactorAuths
                    .FirstOrDefaultAsync(t => t.UserId == userId);

                if (twoFactorAuth == null)
                    return false;

                twoFactorAuth.FailedAttempts++;

                if (twoFactorAuth.FailedAttempts >= MAX_FAILED_ATTEMPTS)
                {
                    twoFactorAuth.LockedUntil = DateTime.UtcNow.AddMinutes(LOCKOUT_DURATION_MINUTES);
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> IsLockedOutAsync(int userId)
        {
            var twoFactorAuth = await _context.TwoFactorAuths
                .FirstOrDefaultAsync(t => t.UserId == userId);

            if (twoFactorAuth?.LockedUntil == null)
                return false;

            if (twoFactorAuth.LockedUntil > DateTime.UtcNow)
                return true;

            // Clear lockout if expired
            twoFactorAuth.LockedUntil = null;
            twoFactorAuth.FailedAttempts = 0;
            await _context.SaveChangesAsync();

            return false;
        }

        private string GenerateSecretKey()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
            var result = new StringBuilder(32);
            using (var rng = RandomNumberGenerator.Create())
            {
                var bytes = new byte[32];
                rng.GetBytes(bytes);
                foreach (var b in bytes)
                {
                    result.Append(chars[b % chars.Length]);
                }
            }
            return result.ToString();
        }

        private string GenerateBackupCode()
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                var bytes = new byte[4];
                rng.GetBytes(bytes);
                return BitConverter.ToUInt32(bytes, 0).ToString("D8");
            }
        }

        private bool VerifyTOTPCode(string secretKey, string code)
        {
            // Simplified TOTP verification - in production use proper TOTP library
            // This is a placeholder implementation
            if (string.IsNullOrEmpty(code) || code.Length != 6)
                return false;

            // For demo purposes, accept any 6-digit code
            // In production, implement proper TOTP algorithm
            return code.All(char.IsDigit);
        }
    }
}
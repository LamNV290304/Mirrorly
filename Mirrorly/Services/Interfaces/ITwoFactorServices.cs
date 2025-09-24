using Mirrorly.Models;

namespace Mirrorly.Services.Interfaces
{
    public interface ITwoFactorServices
    {
        Task<bool> Enable2FAAsync(int userId, string verificationCode);
        Task<bool> Disable2FAAsync(int userId);
        Task<bool> Verify2FACodeAsync(int userId, string code);
        Task<bool> VerifyBackupCodeAsync(int userId, string backupCode);
        Task<string> GenerateSecretKeyAsync(int userId);
        Task<List<string>> GenerateBackupCodesAsync();
        Task<TwoFactorAuth?> Get2FASettingsAsync(int userId);
        Task<bool> Is2FAEnabledAsync(int userId);
        Task<bool> RecordFailedAttemptAsync(int userId);
        Task<bool> IsLockedOutAsync(int userId);
    }
}
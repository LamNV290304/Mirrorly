using System.ComponentModel.DataAnnotations;

namespace Mirrorly.Models
{
    public class TwoFactorAuth
    {
        public int TwoFactorId { get; set; }
        public int UserId { get; set; }
        public bool IsEnabled { get; set; } = false;
        public string? SecretKey { get; set; }
        public string? BackupCodes { get; set; } // JSON array of backup codes
        public DateTime? EnabledAt { get; set; }
        public DateTime? LastUsedAt { get; set; }
        public int FailedAttempts { get; set; } = 0;
        public DateTime? LockedUntil { get; set; }

        // Navigation property
        public virtual User User { get; set; } = null!;
    }
}
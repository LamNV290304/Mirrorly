using System;
using System.Collections.Generic;

namespace Mirrorly.Models;

public partial class TwoFactorAuth
{
    public int TwoFactorId { get; set; }

    public int UserId { get; set; }

    public bool IsEnabled { get; set; }

    public string? SecretKey { get; set; }

    public string? BackupCodes { get; set; }

    public DateTime? EnabledAt { get; set; }

    public DateTime? LastUsedAt { get; set; }

    public int FailedAttempts { get; set; }

    public DateTime? LockedUntil { get; set; }

    public virtual User User { get; set; } = null!;
}

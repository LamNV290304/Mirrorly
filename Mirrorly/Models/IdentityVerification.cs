using System;
using System.Collections.Generic;

namespace Mirrorly.Models;

public partial class IdentityVerification
{
    public long VerificationId { get; set; }

    public int UserId { get; set; }

    public string FullName { get; set; } = null!;

    public string IdNumber { get; set; } = null!;

    public DateTime DateOfBirth { get; set; }

    public string Address { get; set; } = null!;

    public string FrontIdImageUrl { get; set; } = null!;

    public string BackIdImageUrl { get; set; } = null!;

    public string? SelfieImageUrl { get; set; }

    public byte Status { get; set; }

    public string? AdminNotes { get; set; }

    public int? ProcessedByAdminId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? ProcessedAt { get; set; }

    public virtual User? ProcessedByAdmin { get; set; }

    public virtual User User { get; set; } = null!;
}

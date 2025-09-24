using System.ComponentModel.DataAnnotations;

namespace Mirrorly.Models
{
    public class IdentityVerification
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
        public byte Status { get; set; } = 0; // 0=Pending, 1=Approved, 2=Rejected
        public string? AdminNotes { get; set; }
        public int? ProcessedByAdminId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ProcessedAt { get; set; }

        // Navigation properties
        public virtual User User { get; set; } = null!;
        public virtual User? ProcessedByAdmin { get; set; }
    }
}
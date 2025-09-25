using Microsoft.EntityFrameworkCore;
using Mirrorly.Models;
using Mirrorly.Services.Interfaces;

namespace Mirrorly.Services
{
    public class VerificationServices : IVerificationServices
    {
        private readonly ProjectExeContext _context;

        public VerificationServices(ProjectExeContext context)
        {
            _context = context;
        }

        public async Task<bool> CreateVerificationRequestAsync(IdentityVerification verification)
        {
            try
            {
                _context.IdentityVerifications.Add(verification);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<IdentityVerification>> GetPendingVerificationsAsync()
        {
            return await _context.IdentityVerifications
                .Include(v => v.User)
                .Where(v => v.Status == 0)
                .OrderBy(v => v.CreatedAt)
                .ToListAsync();
        }

        public async Task<IdentityVerification?> GetVerificationByIdAsync(long verificationId)
        {
            return await _context.IdentityVerifications
                .Include(v => v.User)
                .Include(v => v.ProcessedByAdmin)
                .FirstOrDefaultAsync(v => v.VerificationId == verificationId);
        }

        public async Task<bool> ProcessVerificationAsync(long verificationId, bool approved, string? adminNotes, int adminId)
        {
            try
            {
                var verification = await _context.IdentityVerifications
                    .FirstOrDefaultAsync(v => v.VerificationId == verificationId);

                if (verification == null || verification.Status != 0)
                    return false;

                verification.Status = approved ? (byte)1 : (byte)2;
                verification.AdminNotes = adminNotes;
                verification.ProcessedByAdminId = adminId;
                verification.ProcessedAt = DateTime.UtcNow;

                // If approved, update user's verification status or add badge
                if (approved)
                {
                    // Could add a IsVerified field to User table or handle differently
                    // For now, we'll use the IdentityVerification record as the source of truth
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> HasVerifiedIdentityAsync(int userId)
        {
            return await _context.IdentityVerifications
                .AnyAsync(v => v.UserId == userId && v.Status == 1);
        }

        public async Task<IdentityVerification?> GetUserVerificationAsync(int userId)
        {
            return await _context.IdentityVerifications
                .Where(v => v.UserId == userId)
                .OrderByDescending(v => v.CreatedAt)
                .FirstOrDefaultAsync();
        }
    }
}
using Mirrorly.Models;

namespace Mirrorly.Services.Interfaces
{
    public interface IVerificationServices
    {
        Task<bool> CreateVerificationRequestAsync(IdentityVerification verification);
        Task<List<IdentityVerification>> GetPendingVerificationsAsync();
        Task<IdentityVerification?> GetVerificationByIdAsync(long verificationId);
        Task<bool> ProcessVerificationAsync(long verificationId, bool approved, string? adminNotes, int adminId);
        Task<bool> HasVerifiedIdentityAsync(int userId);
        Task<IdentityVerification?> GetUserVerificationAsync(int userId);
    }
}
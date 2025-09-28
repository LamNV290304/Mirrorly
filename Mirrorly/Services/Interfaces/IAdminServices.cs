using Mirrorly.Models;

namespace Mirrorly.Services.Interfaces
{
    public interface IAdminServices
    {
        // Dashboard
        Task<AdminDashboardStats> GetDashboardStatsAsync();
        Task<List<User>> GetRecentUsersAsync(int count);

        // User Management
        Task<UserFilterResult> GetFilteredUsersAsync(UserFilterDto filter);
        Task<UserDetailsDto?> GetUserDetailsAsync(int userId);
        Task<bool> ToggleUserStatusAsync(int userId, bool isActive);
        Task<bool> ResetUserPasswordAsync(int userId);
        Task<bool> ForceLogoutUserAsync(int userId);
        Task<bool> ResendEmailVerificationAsync(int userId);
        Task<byte[]> ExportUsersAsync();
        Task<List<int>> GetVerifiedMuaIdsAsync();

        // Booking Management
        Task<List<Booking>> GetRecentBookingsAsync(int count);
        Task<BookingFilterResult> GetFilteredBookingsAsync(BookingFilterDto filter);
        Task<bool> UpdateBookingStatusAsync(long bookingId, byte status);

        // Verification Management
        Task<List<IdentityVerification>> GetPendingVerificationsAsync();
        Task<bool> ProcessVerificationAsync(long verificationId, bool approved, string? notes);

        // Statistics
        Task<AdminReportDto> GenerateReportAsync(DateTime startDate, DateTime endDate);
    }

    // DTOs
    public class AdminDashboardStats
    {
        public int TotalUsers { get; set; }
        public int NewUsersThisMonth { get; set; }
        public int TotalMuas { get; set; }
        public int VerifiedMuas { get; set; }
        public int TotalBookings { get; set; }
        public int BookingsThisMonth { get; set; }
        public int BookingsToday { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal RevenueThisMonth { get; set; }
        public int PendingVerifications { get; set; }
        public int PendingReports { get; set; }
    }

    public class UserFilterDto
    {
        public string SearchTerm { get; set; } = "";
        public int? RoleId { get; set; }
        public string Status { get; set; } = "";
        public string SortBy { get; set; } = "newest";
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    public class UserFilterResult
    {
        public List<User> Users { get; set; } = new();
        public int TotalUsers { get; set; }
        public int FilteredCount { get; set; }
    }

    public class UserDetailsDto
    {
        public User User { get; set; } = null!;
        public Muaprofile? MuaProfile { get; set; }
        public int TotalBookings { get; set; }
        public int TotalReviews { get; set; }
        public int TotalServices { get; set; }
        public int TotalPortfolioItems { get; set; }
        public bool IsVerified { get; set; }
    }

    public class BookingFilterDto
    {
        public string SearchTerm { get; set; } = "";
        public byte? Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string SortBy { get; set; } = "newest";
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    public class BookingFilterResult
    {
        public List<Booking> Bookings { get; set; } = new();
        public int TotalBookings { get; set; }
        public int FilteredCount { get; set; }
    }

    public class AdminReportDto
    {
        public int TotalUsers { get; set; }
        public int NewUsers { get; set; }
        public int TotalBookings { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalReviews { get; set; }
        public double AverageRating { get; set; }
        public List<PopularService> PopularServices { get; set; } = new();
        public List<TopMua> TopMuas { get; set; } = new();
    }

    public class PopularService
    {
        public string ServiceName { get; set; } = "";
        public int BookingCount { get; set; }
        public decimal Revenue { get; set; }
    }

    public class TopMua
    {
        public string MuaName { get; set; } = "";
        public int BookingCount { get; set; }
        public decimal Revenue { get; set; }
        public double Rating { get; set; }
    }

    public class ToggleUserStatusDto
    {
        public int UserId { get; set; }
        public bool IsActive { get; set; }
    }

    public class UserActionDto
    {
        public int UserId { get; set; }
    }
}
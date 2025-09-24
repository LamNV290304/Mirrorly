using Microsoft.EntityFrameworkCore;
using Mirrorly.Models;
using Mirrorly.Services.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace Mirrorly.Services
{
    public class AdminServices : IAdminServices
    {
        private readonly ProjectExeContext _context;
        private readonly IAuthServices _authServices;

        public AdminServices(ProjectExeContext context, IAuthServices authServices)
        {
            _context = context;
            _authServices = authServices;
        }

        public async Task<AdminDashboardStats> GetDashboardStatsAsync()
        {
            var now = DateTime.Now;
            var startOfMonth = new DateTime(now.Year, now.Month, 1);
            var startOfToday = now.Date;

            var stats = new AdminDashboardStats();

            // User statistics
            stats.TotalUsers = await _context.Users.CountAsync();
            stats.NewUsersThisMonth = await _context.Users
                .CountAsync(u => u.UserId > 0); // Placeholder - add CreatedAt field

            stats.TotalMuas = await _context.Users.CountAsync(u => u.RoleId == 2);
            stats.VerifiedMuas = await _context.Muaprofiles
                .CountAsync(m => m.ProfilePublic); // Using ProfilePublic as verified indicator

            // Booking statistics
            stats.TotalBookings = await _context.Bookings.CountAsync();
            stats.BookingsThisMonth = await _context.Bookings
                .CountAsync(b => b.ScheduledStart >= startOfMonth);
            stats.BookingsToday = await _context.Bookings
                .CountAsync(b => b.ScheduledStart >= startOfToday && b.ScheduledStart < startOfToday.AddDays(1));

            // Revenue statistics (placeholder calculations)
            var bookings = await _context.Bookings
                .Include(b => b.BookingItems)
                .ToListAsync();

            stats.TotalRevenue = bookings.Sum(b => b.BookingItems.Sum(bi => bi.UnitPrice * bi.Quantity));
            stats.RevenueThisMonth = bookings
                .Where(b => b.ScheduledStart >= startOfMonth)
                .Sum(b => b.BookingItems.Sum(bi => bi.UnitPrice * bi.Quantity));

            // Pending items
            stats.PendingVerifications = 5; // Placeholder
            stats.PendingReports = 2; // Placeholder

            return stats;
        }

        public async Task<List<User>> GetRecentUsersAsync(int count)
        {
            return await _context.Users
                .Include(u => u.Role)
                .OrderByDescending(u => u.UserId) // Placeholder for CreatedAt
                .Take(count)
                .ToListAsync();
        }

        public async Task<UserFilterResult> GetFilteredUsersAsync(UserFilterDto filter)
        {
            var query = _context.Users.Include(u => u.Role).AsQueryable();

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var search = filter.SearchTerm.ToLower();
                query = query.Where(u =>
                    u.FullName.ToLower().Contains(search) ||
                    u.Email.ToLower().Contains(search) ||
                    u.Username.ToLower().Contains(search));
            }

            // Apply role filter
            if (filter.RoleId.HasValue)
            {
                query = query.Where(u => u.RoleId == filter.RoleId.Value);
            }

            // Apply status filter
            if (!string.IsNullOrEmpty(filter.Status))
            {
                switch (filter.Status)
                {
                    case "active":
                        query = query.Where(u => u.IsActive);
                        break;
                    case "inactive":
                        query = query.Where(u => !u.IsActive);
                        break;
                    case "verified":
                        query = query.Where(u => u.IsEmailVerified);
                        break;
                }
            }

            var totalUsers = await _context.Users.CountAsync();
            var filteredCount = await query.CountAsync();

            // Apply sorting
            query = filter.SortBy switch
            {
                "oldest" => query.OrderBy(u => u.UserId),
                "name" => query.OrderBy(u => u.FullName),
                "email" => query.OrderBy(u => u.Email),
                _ => query.OrderByDescending(u => u.UserId)
            };

            // Apply pagination
            var users = await query
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new UserFilterResult
            {
                Users = users,
                TotalUsers = totalUsers,
                FilteredCount = filteredCount
            };
        }

        public async Task<UserDetailsDto?> GetUserDetailsAsync(int userId)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .Include(u => u.Muaprofile)
                .Include(u => u.CustomerProfile)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null) return null;

            var details = new UserDetailsDto
            {
                User = user,
                MuaProfile = user.Muaprofile
            };

            if (user.RoleId == 1) // Customer
            {
                details.TotalBookings = await _context.Bookings
                    .CountAsync(b => b.CustomerId == userId);
                details.TotalReviews = await _context.Reviews
                    .CountAsync(r => r.CustomerId == userId);
            }
            else if (user.RoleId == 2) // MUA
            {
                details.TotalBookings = await _context.Bookings
                    .CountAsync(b => b.MuaId == userId);
                details.TotalReviews = await _context.Reviews
                    .CountAsync(r => r.MuaId == userId);
                details.TotalServices = await _context.Services
                    .CountAsync(s => s.MuaId == userId);
                details.TotalPortfolioItems = await _context.PortfolioItems
                    .CountAsync(p => p.Muaid == userId);
                details.IsVerified = user.Muaprofile?.ProfilePublic ?? false;
            }

            return details;
        }

        public async Task<bool> ToggleUserStatusAsync(int userId, bool isActive)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null) return false;

                user.IsActive = isActive;
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ResetUserPasswordAsync(int userId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null) return false;

                // Generate new password
                var newPassword = GenerateRandomPassword();
                user.PasswordHash = _authServices.HashPassword(newPassword);

                await _context.SaveChangesAsync();

                // TODO: Send email with new password
                // await _emailService.SendPasswordResetEmail(user.Email, newPassword);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ForceLogoutUserAsync(int userId)
        {
            try
            {
                // TODO: Implement session invalidation
                // This would typically involve updating a session token or version
                // For now, just return true as placeholder
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ResendEmailVerificationAsync(int userId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null) return false;

                // TODO: Send verification email
                // await _emailService.SendVerificationEmail(user.Email);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<byte[]> ExportUsersAsync()
        {
            // TODO: Implement Excel export using EPPlus or similar library
            // For now, return empty byte array
            return new byte[0];
        }

        public async Task<List<int>> GetVerifiedMuaIdsAsync()
        {
            return await _context.Muaprofiles
                .Where(m => m.ProfilePublic)
                .Select(m => m.Muaid)
                .ToListAsync();
        }

        public async Task<List<Booking>> GetRecentBookingsAsync(int count)
        {
            return await _context.Bookings
                .Include(b => b.Customer)
                .Include(b => b.Mua)
                .OrderByDescending(b => b.BookingId)
                .Take(count)
                .ToListAsync();
        }

        public async Task<BookingFilterResult> GetFilteredBookingsAsync(BookingFilterDto filter)
        {
            var query = _context.Bookings
                .Include(b => b.Customer)
                .Include(b => b.Mua)
                .Include(b => b.BookingItems)
                .ThenInclude(bi => bi.Service)
                .AsQueryable();

            // Apply filters
            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var search = filter.SearchTerm.ToLower();
                query = query.Where(b =>
                    b.Customer.DisplayName.ToLower().Contains(search) ||
                    b.Mua.DisplayName.ToLower().Contains(search));
            }

            if (filter.Status.HasValue)
            {
                query = query.Where(b => b.Status == filter.Status.Value);
            }

            if (filter.StartDate.HasValue)
            {
                query = query.Where(b => b.ScheduledStart >= filter.StartDate.Value);
            }

            if (filter.EndDate.HasValue)
            {
                query = query.Where(b => b.ScheduledStart <= filter.EndDate.Value);
            }

            var totalBookings = await _context.Bookings.CountAsync();
            var filteredCount = await query.CountAsync();

            // Apply sorting and pagination
            query = filter.SortBy switch
            {
                "oldest" => query.OrderBy(b => b.ScheduledStart),
                "customer" => query.OrderBy(b => b.Customer.DisplayName),
                "mua" => query.OrderBy(b => b.Mua.DisplayName),
                _ => query.OrderByDescending(b => b.ScheduledStart)
            };

            var bookings = await query
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new BookingFilterResult
            {
                Bookings = bookings,
                TotalBookings = totalBookings,
                FilteredCount = filteredCount
            };
        }

        public async Task<bool> UpdateBookingStatusAsync(long bookingId, byte status)
        {
            try
            {
                var booking = await _context.Bookings.FindAsync(bookingId);
                if (booking == null) return false;

                booking.Status = status;
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
            // TODO: Implement when IdentityVerification model is created
            return new List<IdentityVerification>();
        }

        public async Task<bool> ProcessVerificationAsync(long verificationId, bool approved, string? notes)
        {
            // TODO: Implement verification processing
            return true;
        }

        public async Task<AdminReportDto> GenerateReportAsync(DateTime startDate, DateTime endDate)
        {
            // TODO: Implement comprehensive reporting
            return new AdminReportDto();
        }

        private string GenerateRandomPassword(int length = 12)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*";
            var result = new StringBuilder(length);
            using (var rng = RandomNumberGenerator.Create())
            {
                var bytes = new byte[length];
                rng.GetBytes(bytes);
                foreach (var b in bytes)
                {
                    result.Append(chars[b % chars.Length]);
                }
            }
            return result.ToString();
        }
    }
}
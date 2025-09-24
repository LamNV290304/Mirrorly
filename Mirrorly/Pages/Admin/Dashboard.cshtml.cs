using Microsoft.AspNetCore.Mvc.RazorPages;
using Mirrorly.Services.Interfaces;
using Mirrorly.Models;

namespace Mirrorly.Pages.Admin
{
    public class DashboardModel : PageModel
    {
        private readonly IAdminServices _adminServices;

        public DashboardModel(IAdminServices adminServices)
        {
            _adminServices = adminServices;
        }

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
        public List<User> RecentUsers { get; set; } = new();

        public async Task OnGetAsync()
        {
            // Check admin permission
            var roleId = HttpContext.Session.GetInt32("RoleId");
            if (roleId != 3)
            {
                Response.Redirect("/Auth/Login");
                return;
            }

            var stats = await _adminServices.GetDashboardStatsAsync();

            TotalUsers = stats.TotalUsers;
            NewUsersThisMonth = stats.NewUsersThisMonth;
            TotalMuas = stats.TotalMuas;
            VerifiedMuas = stats.VerifiedMuas;
            TotalBookings = stats.TotalBookings;
            BookingsThisMonth = stats.BookingsThisMonth;
            BookingsToday = stats.BookingsToday;
            TotalRevenue = stats.TotalRevenue;
            RevenueThisMonth = stats.RevenueThisMonth;
            PendingVerifications = stats.PendingVerifications;
            PendingReports = stats.PendingReports;
            RecentUsers = await _adminServices.GetRecentUsersAsync(10);
        }
    }
}
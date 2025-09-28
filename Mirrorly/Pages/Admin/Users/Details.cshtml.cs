using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Mirrorly.Services.Interfaces;
using Mirrorly.Models;

namespace Mirrorly.Pages.Admin.Users
{
    public class DetailsModel : PageModel
    {
        private readonly IAdminServices _adminServices;

        public DetailsModel(IAdminServices adminServices)
        {
            _adminServices = adminServices;
        }

        public User User { get; set; } = null!;
        public Muaprofile? MuaProfile { get; set; }
        public int TotalBookings { get; set; }
        public int TotalReviews { get; set; }
        public int TotalServices { get; set; }
        public int TotalPortfolioItems { get; set; }
        public bool IsVerified { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var roleId = HttpContext.Session.GetInt32("RoleId");
            if (roleId != 3) return RedirectToPage("/Auth/Login");

            var userDetails = await _adminServices.GetUserDetailsAsync(id);
            if (userDetails == null) return NotFound();

            User = userDetails.User;
            MuaProfile = userDetails.MuaProfile;
            TotalBookings = userDetails.TotalBookings;
            TotalReviews = userDetails.TotalReviews;
            TotalServices = userDetails.TotalServices;
            TotalPortfolioItems = userDetails.TotalPortfolioItems;
            IsVerified = userDetails.IsVerified;

            return Page();
        }

        public async Task<IActionResult> OnPostResetPasswordAsync([FromBody] UserActionDto request)
        {
            var roleId = HttpContext.Session.GetInt32("RoleId");
            if (roleId != 3) return Forbid();

            var success = await _adminServices.ResetUserPasswordAsync(request.UserId);
            return new JsonResult(new { success });
        }

        public async Task<IActionResult> OnPostForceLogoutAsync([FromBody] UserActionDto request)
        {
            var roleId = HttpContext.Session.GetInt32("RoleId");
            if (roleId != 3) return Forbid();

            var success = await _adminServices.ForceLogoutUserAsync(request.UserId);
            return new JsonResult(new { success });
        }

        public async Task<IActionResult> OnPostResendVerificationAsync([FromBody] UserActionDto request)
        {
            var roleId = HttpContext.Session.GetInt32("RoleId");
            if (roleId != 3) return Forbid();

            var success = await _adminServices.ResendEmailVerificationAsync(request.UserId);
            return new JsonResult(new { success });
        }
    }
}
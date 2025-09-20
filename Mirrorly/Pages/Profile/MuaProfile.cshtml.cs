using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Mirrorly.Services.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Mirrorly.Pages.Profile
{
    public class MuaProfileModel : PageModel
    {
        private readonly IProfileServices _profileServices;
        private readonly IAuthServices _authServices;

        public MuaProfileModel(IProfileServices profileServices, IAuthServices authServices)
        {
            _profileServices = profileServices;
            _authServices = authServices;
        }

        [BindProperty]
        [Required(ErrorMessage = "Tên hiển thị là bắt buộc")]
        [StringLength(120, ErrorMessage = "Tên hiển thị không được quá 120 ký tự")]
        public string DisplayName { get; set; } = "";

        [BindProperty]
        public string? Bio { get; set; }

        [BindProperty]
        [StringLength(255, ErrorMessage = "Địa chỉ không được quá 255 ký tự")]
        public string? AddressLine { get; set; }

        [BindProperty]
        [Range(0, 50, ErrorMessage = "Số năm kinh nghiệm phải từ 0-50 năm")]
        public int? ExperienceYears { get; set; }

        [BindProperty]
        public bool ProfilePublic { get; set; }

        public string Email { get; set; } = "";
        public string Username { get; set; } = "";

        [TempData]
        public string? SuccessMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToPage("/Auth/Login");
            }

            var user = await _authServices.GetUserById(userId.Value);
            if (user == null || user.RoleId != 2)
            {
                return RedirectToPage("/Auth/Login");
            }

            var muaProfile = await _profileServices.GetMuaProfile(userId.Value);
            if (muaProfile != null)
            {
                DisplayName = muaProfile.DisplayName;
                Bio = muaProfile.Bio;
                AddressLine = muaProfile.AddressLine;
                ExperienceYears = muaProfile.ExperienceYears;
                ProfilePublic = muaProfile.ProfilePublic;
            }
            else
            {
                // Set default values for new profile
                DisplayName = user.FullName ?? "";
                ProfilePublic = false;
            }

            Email = user.Email;
            Username = user.Username;

            return Page();
        }

        public async Task<IActionResult> OnPostUpdateBasicInfoAsync()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToPage("/Auth/Login");
            }

            // Validate required fields manually since we only want to validate basic info
            if (string.IsNullOrWhiteSpace(DisplayName))
            {
                ModelState.AddModelError("DisplayName", "Tên hiển thị là bắt buộc");
            }

            if (!ModelState.IsValid)
            {
                // Reload user info for display
                var user = await _authServices.GetUserById(userId.Value);
                if (user != null)
                {
                    Email = user.Email;
                    Username = user.Username;
                }
                return Page();
            }

            var muaProfile = await _profileServices.GetMuaProfile(userId.Value);

            if (muaProfile == null)
            {
                // Create new profile if doesn't exist
                muaProfile = new Mirrorly.Models.Muaprofile
                {
                    Muaid = userId.Value,
                    DisplayName = DisplayName,
                    Bio = Bio,
                    AddressLine = AddressLine,
                    ExperienceYears = ExperienceYears,
                    ProfilePublic = ProfilePublic
                };

                var createSuccess = await _profileServices.CreateMuaProfile(muaProfile);
                if (createSuccess)
                {
                    TempData["SuccessMessage"] = "Hồ sơ đã được tạo thành công!";
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Có lỗi xảy ra khi tạo hồ sơ");
                    return Page();
                }
            }
            else
            {
                // Update existing profile
                muaProfile.DisplayName = DisplayName;
                muaProfile.Bio = Bio;
                muaProfile.AddressLine = AddressLine;
                muaProfile.ExperienceYears = ExperienceYears;
                muaProfile.ProfilePublic = ProfilePublic;

                var updateSuccess = await _profileServices.UpdateMuaProfile(muaProfile);
                if (updateSuccess)
                {
                    TempData["SuccessMessage"] = "Hồ sơ đã được cập nhật thành công!";
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Có lỗi xảy ra khi cập nhật hồ sơ");
                    return Page();
                }
            }

            return RedirectToPage();
        }
    }
}
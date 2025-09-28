using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Mirrorly.Models;
using Mirrorly.Services.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Mirrorly.Pages.Profile
{
    public class CustomerProfileModel : BasePageModel
    {
        private readonly IProfileServices _profileServices;
        private readonly IAuthServices _authServices;

        public CustomerProfileModel(IProfileServices profileServices, IAuthServices authServices,
            ProjectExeContext context, IVerificationServices verificationServices, ITwoFactorServices twoFactorServices)
            : base(context, verificationServices, twoFactorServices)
        {
            _profileServices = profileServices;
            _authServices = authServices;
        }

        [BindProperty]
        [StringLength(120, ErrorMessage = "Tên hiển thị không được quá 120 ký tự")]
        public string? DisplayName { get; set; }

        [BindProperty]
        [StringLength(30, ErrorMessage = "Số điện thoại không được quá 30 ký tự")]
        public string? PhoneNumber { get; set; }

        [BindProperty]
        [StringLength(500, ErrorMessage = "URL avatar không được quá 500 ký tự")]
        [Url(ErrorMessage = "URL không hợp lệ")]
        public string? AvatarUrl { get; set; }

        public string Email { get; set; } = "";
        public string Username { get; set; } = "";

        [TempData]
        public string? SuccessMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (!RequireCustomer()) return Page();

            var user = await _authServices.GetUserById(CurrentUserId!.Value);
            if (user == null) return RedirectToPage("/Auth/Login");

            var customerProfile = await _profileServices.GetCustomerProfile(CurrentUserId.Value);
            if (customerProfile != null)
            {
                DisplayName = customerProfile.DisplayName;
                PhoneNumber = customerProfile.PhoneNumber;
                AvatarUrl = customerProfile.AvatarUrl;
            }

            Email = user.Email;
            Username = user.Username;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!RequireCustomer()) return Page();

            if (!ModelState.IsValid)
            {
                // Reload user info for display
                var user = await _authServices.GetUserById(CurrentUserId!.Value);
                if (user != null)
                {
                    Email = user.Email;
                    Username = user.Username;
                }
                return Page();
            }

            var customerProfile = await _profileServices.GetCustomerProfile(CurrentUserId.Value);

            if (customerProfile == null)
            {
                // Create new profile if doesn't exist
                customerProfile = new CustomerProfile
                {
                    CustomerId = CurrentUserId.Value,
                    DisplayName = DisplayName,
                    PhoneNumber = PhoneNumber,
                    AvatarUrl = AvatarUrl
                };

                var createSuccess = await _profileServices.CreateCustomerProfile(customerProfile);
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
                customerProfile.DisplayName = DisplayName;
                customerProfile.PhoneNumber = PhoneNumber;
                customerProfile.AvatarUrl = AvatarUrl;

                var updateSuccess = await _profileServices.UpdateCustomerProfile(customerProfile);
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
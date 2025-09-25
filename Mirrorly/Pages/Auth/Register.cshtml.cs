using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Mirrorly.Models;
using Mirrorly.Services.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Mirrorly.Pages.Auth
{
    public class RegisterModel : PageModel
    {
        private readonly IAuthServices _authServices;
        private readonly IProfileServices _profileServices;

        public RegisterModel(IAuthServices authServices, IProfileServices profileServices)
        {
            _authServices = authServices;
            _profileServices = profileServices;
        }

        [BindProperty]
        [Required(ErrorMessage = "Tên đăng nhập là bắt buộc")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Tên đăng nhập phải từ 3-50 ký tự")]
        public string Username { get; set; } = "";

        [BindProperty]
        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; } = "";

        [BindProperty]
        [Required(ErrorMessage = "Họ tên là bắt buộc")]
        [StringLength(100, ErrorMessage = "Họ tên không được quá 100 ký tự")]
        public string FullName { get; set; } = "";

        [BindProperty]
        [StringLength(30, ErrorMessage = "Số điện thoại không được quá 30 ký tự")]
        public string? PhoneNumber { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải ít nhất 6 ký tự")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = "";

        [BindProperty]
        [Required(ErrorMessage = "Xác nhận mật khẩu là bắt buộc")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp")]
        public string ConfirmPassword { get; set; } = "";

        [BindProperty]
        [Required(ErrorMessage = "Vui lòng chọn loại tài khoản")]
        public byte RoleId { get; set; }

        [TempData]
        public string? ErrorMessage { get; set; }

        [TempData]
        public string? SuccessMessage { get; set; }

        public void OnGet()
        {
            // Check if user is already logged in
            if (HttpContext.Session.GetInt32("UserId").HasValue)
            {
                Response.Redirect("/");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Validate role
            if (RoleId != 1 && RoleId != 2)
            {
                ModelState.AddModelError("RoleId", "Loại tài khoản không hợp lệ");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Check if email exists
            if (await _authServices.IsEmailExists(Email))
            {
                ModelState.AddModelError("Email", "Email đã được sử dụng");
                return Page();
            }

            // Check if username exists
            if (await _authServices.IsUsernameExists(Username))
            {
                ModelState.AddModelError("Username", "Tên đăng nhập đã được sử dụng");
                return Page();
            }

            // Create user
            var user = new User
            {
                Username = Username,
                Email = Email,
                FullName = FullName,
                PhoneNumber = PhoneNumber
            };

            var success = await _authServices.Register(user, Password, RoleId);

            if (success)
            {
                // Get the created user to get the UserId
                var createdUser = await _authServices.GetUserByEmail(Email);

                if (createdUser != null)
                {
                    // Create profile based on role
                    if (RoleId == 1) // Customer
                    {
                        var customerProfile = new CustomerProfile
                        {
                            CustomerId = createdUser.UserId,
                            DisplayName = FullName,
                            PhoneNumber = PhoneNumber
                        };
                        await _profileServices.CreateCustomerProfile(customerProfile);
                    }
                    else if (RoleId == 2) // MUA
                    {
                        var muaProfile = new Muaprofile
                        {
                            Muaid = createdUser.UserId,
                            DisplayName = FullName,
                            ProfilePublic = false // Initially private until completed
                        };
                        await _profileServices.CreateMuaProfile(muaProfile);
                    }
                }

                TempData["SuccessMessage"] = "Đăng ký thành công! Vui lòng đăng nhập.";
                return RedirectToPage("/Auth/Login");
            }

            ModelState.AddModelError(string.Empty, "Đã có lỗi xảy ra trong quá trình đăng ký");
            return Page();
        }
    }
}
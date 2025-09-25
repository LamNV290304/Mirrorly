// Pages/Auth/Login.cshtml.cs - Updated with 2FA
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Mirrorly.Services.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Mirrorly.Pages.Auth
{
    public class LoginModel : PageModel
    {
        private readonly IAuthServices _authServices;
        private readonly ITwoFactorServices _twoFactorServices;

        public LoginModel(IAuthServices authServices, ITwoFactorServices twoFactorServices)
        {
            _authServices = authServices;
            _twoFactorServices = twoFactorServices;
        }

        [BindProperty]
        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; } = "";

        [BindProperty]
        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = "";

        //[BindProperty]
        public string TwoFactorCode { get; set; } = "";

        //[BindProperty]
        public string BackupCode { get; set; } = "";

        [BindProperty]
        public int TempUserId { get; set; }

        public bool RequiresTwoFactor { get; set; } = false;

        [TempData]
        public string? ErrorMessage { get; set; }

        [TempData]
        public string? SuccessMessage { get; set; }

        public void OnGet()
        {
            // Check if user is already logged in
            if (HttpContext.Session.GetInt32("UserId").HasValue)
            {
                RedirectBasedOnRole();
            }

            // Check if this is a 2FA step
            RequiresTwoFactor = TempData.ContainsKey("RequiresTwoFactor");
            if (RequiresTwoFactor)
            {
                Email = TempData["TwoFactorEmail"]?.ToString() ?? "";
                TempUserId = int.Parse(TempData["TempUserId"]?.ToString() ?? "0");

                // Keep TempData for next request
                TempData.Keep("RequiresTwoFactor");
                TempData.Keep("TwoFactorEmail");
                TempData.Keep("TempUserId");
            }
        }

        public async Task<IActionResult> OnPostBasicLoginAsync()
        {
            foreach (var kv in ModelState)
            {
                foreach (var error in kv.Value.Errors)
                {
                    Console.WriteLine($"Field {kv.Key}: {error.ErrorMessage}");
                }
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var user = await _authServices.Login(Email, Password);

                if (user != null)
                {
                    // Check if user has 2FA enabled
                    //var has2FA = await _twoFactorServices.Is2FAEnabledAsync(user.UserId);
                    var has2FA = false;

                    if (has2FA)
                    {
                        // Don't log in yet, require 2FA verification
                        TempData["RequiresTwoFactor"] = true;
                        TempData["TwoFactorEmail"] = Email;
                        TempData["TempUserId"] = user.UserId;

                        RequiresTwoFactor = true;
                        TempUserId = user.UserId;

                        return Page();
                    }
                    else
                    {
                        // Login without 2FA
                        SetUserSession(user);
                        return RedirectBasedOnRole(user.RoleId);
                    }
                }

                ModelState.AddModelError(string.Empty, "Email hoặc mật khẩu không chính xác");
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Có lỗi xảy ra trong quá trình đăng nhập");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostTwoFactorLoginAsync()
        {
            if (TempUserId == 0 || string.IsNullOrEmpty(TwoFactorCode))
            {
                ModelState.AddModelError(string.Empty, "Mã xác thực không hợp lệ");
                RequiresTwoFactor = true;
                return Page();
            }

            try
            {
                // Check if account is locked out
                var isLockedOut = await _twoFactorServices.IsLockedOutAsync(TempUserId);
                if (isLockedOut)
                {
                    ErrorMessage = "Tài khoản tạm thời bị khóa do nhập sai mã quá nhiều lần. Vui lòng thử lại sau 15 phút.";
                    RequiresTwoFactor = true;
                    return Page();
                }

                // Verify 2FA code
                var isValidCode = await _twoFactorServices.Verify2FACodeAsync(TempUserId, TwoFactorCode);

                if (isValidCode)
                {
                    // Get user and complete login
                    var user = await _authServices.GetUserById(TempUserId);
                    if (user != null)
                    {
                        SetUserSession(user);

                        // Clear TempData
                        TempData.Remove("RequiresTwoFactor");
                        TempData.Remove("TwoFactorEmail");
                        TempData.Remove("TempUserId");

                        SuccessMessage = "Đăng nhập thành công với xác thực hai yếu tố!";
                        return RedirectBasedOnRole(user.RoleId);
                    }
                }

                // Failed verification - record attempt and show error
                await _twoFactorServices.RecordFailedAttemptAsync(TempUserId);

                var remainingAttempts = 5 - (await GetFailedAttemptsCount(TempUserId));
                ErrorMessage = $"Mã xác thực không chính xác. Còn {remainingAttempts} lần thử.";

                RequiresTwoFactor = true;
                return Page();
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Có lỗi xảy ra khi xác minh mã 2FA");
                RequiresTwoFactor = true;
                return Page();
            }
        }

        public async Task<IActionResult> OnPostBackupCodeLoginAsync()
        {
            if (TempUserId == 0 || string.IsNullOrEmpty(BackupCode))
            {
                ModelState.AddModelError(string.Empty, "Recovery code không hợp lệ");
                RequiresTwoFactor = true;
                return Page();
            }

            try
            {
                // Verify backup code
                var isValidBackupCode = await _twoFactorServices.VerifyBackupCodeAsync(TempUserId, BackupCode);

                if (isValidBackupCode)
                {
                    // Get user and complete login
                    var user = await _authServices.GetUserById(TempUserId);
                    if (user != null)
                    {
                        SetUserSession(user);

                        // Clear TempData
                        TempData.Remove("RequiresTwoFactor");
                        TempData.Remove("TwoFactorEmail");
                        TempData.Remove("TempUserId");

                        SuccessMessage = "Đăng nhập thành công bằng recovery code! Khuyến nghị tạo mã recovery mới.";
                        return RedirectBasedOnRole(user.RoleId);
                    }
                }

                ErrorMessage = "Recovery code không chính xác hoặc đã được sử dụng";
                RequiresTwoFactor = true;
                return Page();
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Có lỗi xảy ra khi xác minh recovery code");
                RequiresTwoFactor = true;
                return Page();
            }
        }

        private void SetUserSession(Mirrorly.Models.User user)
        {
            HttpContext.Session.SetInt32("UserId", user.UserId);
            HttpContext.Session.SetString("Username", user.Username);
            HttpContext.Session.SetString("Email", user.Email);
            HttpContext.Session.SetString("FullName", user.FullName ?? "");
            HttpContext.Session.SetInt32("RoleId", user.RoleId);
        }

        private IActionResult RedirectBasedOnRole(byte? roleId = null)
        {
            var role = roleId ?? HttpContext.Session.GetInt32("RoleId");

            return role switch
            {
                3 => RedirectToPage("/Admin/Dashboard"), // Admin
                2 => RedirectToPage("/Profile/MuaProfile"), // MUA
                1 => RedirectToPage("/Profile/CustomerProfile"), // Customer
                _ => RedirectToPage("/")
            };
        }

        private async Task<int> GetFailedAttemptsCount(int userId)
        {
            var twoFactorAuth = await _twoFactorServices.Get2FASettingsAsync(userId);
            return twoFactorAuth?.FailedAttempts ?? 0;
        }
    }
}
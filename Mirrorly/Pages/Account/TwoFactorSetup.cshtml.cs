using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Mirrorly.Models;
using Mirrorly.Services.Interfaces;
using System.Security.Cryptography;
using System.Text.Json;

namespace Mirrorly.Pages.Account
{
    public class TwoFactorSetupModel : PageModel
    {
        private readonly ProjectExeContext _context;
        private readonly ITwoFactorServices _twoFactorServices;

        public TwoFactorSetupModel(ProjectExeContext context, ITwoFactorServices twoFactorServices)
        {
            _context = context;
            _twoFactorServices = twoFactorServices;
        }

        public User User { get; set; } = null!;
        public TwoFactorAuth? TwoFactorAuth { get; set; }
        public bool Is2FAEnabled => TwoFactorAuth?.IsEnabled ?? false;
        public List<string>? BackupCodes { get; set; }

        [TempData]
        public string? SuccessMessage { get; set; }

        [TempData]
        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToPage("/Auth/Login");
            }

            User = await _context.Users.FindAsync(userId.Value);
            if (User == null)
            {
                return RedirectToPage("/Auth/Login");
            }

            TwoFactorAuth = await _context.TwoFactorAuths
                .FirstOrDefaultAsync(t => t.UserId == userId.Value);

            if (TwoFactorAuth?.IsEnabled == true && !string.IsNullOrEmpty(TwoFactorAuth.BackupCodes))
            {
                BackupCodes = JsonSerializer.Deserialize<List<string>>(TwoFactorAuth.BackupCodes);
            }

            return Page();
        }

        public async Task<IActionResult> OnPostEnable2FAAsync(string verificationCode)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToPage("/Auth/Login");
            }

            if (string.IsNullOrEmpty(verificationCode) || verificationCode.Length != 6)
            {
                ErrorMessage = "Vui lòng nhập mã 6 chữ số hợp lệ";
                return RedirectToPage();
            }

            try
            {
                var success = await _twoFactorServices.Enable2FAAsync(userId.Value, verificationCode);

                if (success)
                {
                    SuccessMessage = "2FA đã được kích hoạt thành công! Hãy lưu trữ recovery codes ở nơi an toàn.";
                }
                else
                {
                    ErrorMessage = "Mã xác minh không chính xác. Vui lòng thử lại.";
                }
            }
            catch (Exception)
            {
                ErrorMessage = "Có lỗi xảy ra khi kích hoạt 2FA. Vui lòng thử lại.";
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDisable2FAAsync()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToPage("/Auth/Login");
            }

            try
            {
                var success = await _twoFactorServices.Disable2FAAsync(userId.Value);

                if (success)
                {
                    SuccessMessage = "2FA đã được tắt thành công.";
                }
                else
                {
                    ErrorMessage = "Có lỗi xảy ra khi tắt 2FA.";
                }
            }
            catch (Exception)
            {
                ErrorMessage = "Có lỗi hệ thống. Vui lòng thử lại sau.";
            }

            return new JsonResult(new { success = true });
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Mirrorly.Services.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Mirrorly.Pages.Auth
{
    public class ChangePasswordModel : PageModel
    {
        private readonly IAuthServices _authServices;

        public ChangePasswordModel(IAuthServices authServices)
        {
            _authServices = authServices;
        }

        [BindProperty]
        [Required(ErrorMessage = "Mật khẩu cũ là bắt buộc")]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; } = "";

        [BindProperty]
        [Required(ErrorMessage = "Mật khẩu mới là bắt buộc")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
        public string NewPassword { get; set; } = "";

        [BindProperty]
        [Required(ErrorMessage = "Xác nhận mật khẩu là bắt buộc")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Mật khẩu xác nhận không khớp")]
        public string ConfirmPassword { get; set; } = "";

        [TempData]
        public string? ErrorMessage { get; set; }

        [TempData]
        public string? SuccessMessage { get; set; }

        public void OnGet()
        {
            if (!HttpContext.Session.GetInt32("UserId").HasValue)
            {
                Response.Redirect("/Auth/Login");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
                return RedirectToPage("/Auth/Login");

            var result = await _authServices.ChangePassword(userId.Value, OldPassword, NewPassword);
            if (result)
            {
                SuccessMessage = "Đổi mật khẩu thành công!";
                return RedirectToPage("/Index");
            }
            else
            {
                ErrorMessage = "Mật khẩu cũ không chính xác.";
                return Page();
            }
        }
    }
}

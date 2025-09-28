using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Mirrorly.Models;
using Mirrorly.Services.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Mirrorly.Pages.Auth
{
    public class LoginModel : PageModel
    {
        private readonly IAuthServices _authServices;

        public LoginModel(IAuthServices authServices)
        {
            _authServices = authServices;
        }

        [BindProperty]
        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; } = "";

        [BindProperty]
        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = "";

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
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _authServices.Login(Email, Password);

            if (user != null)
            {
                // Set session
                HttpContext.Session.SetInt32("UserId", user.UserId);
                HttpContext.Session.SetString("Username", user.Username);
                HttpContext.Session.SetString("Email", user.Email);
                HttpContext.Session.SetString("FullName", user.FullName ?? "");
                HttpContext.Session.SetInt32("RoleId", user.RoleId);

                // Redirect based on role
                if (user.RoleId == 2) // MUA
                {
                    return RedirectToPage("/Profile/MuaProfile");
                }
                else // Customer
                {
                    return RedirectToPage("/Profile/CustomerProfile");
                }
            }

            ModelState.AddModelError(string.Empty, "Email hoặc mật khẩu không chính xác");
            return Page();
        }
    }
}
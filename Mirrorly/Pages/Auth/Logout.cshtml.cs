// Pages/Auth/Logout.cshtml.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Mirrorly.Pages.Auth
{
    public class LogoutModel : PageModel
    {
        public IActionResult OnGet()
        {
            // Clear session
            HttpContext.Session.Clear();

            // Set success message
            TempData["SuccessMessage"] = "Đăng xuất thành công!";

            // Redirect to login page
            return RedirectToPage("/Auth/Login");
        }

        public IActionResult OnPost()
        {
            return OnGet();
        }
    }
}
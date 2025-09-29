using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Mirrorly.Pages.Payment
{
    public class UpgradeModel : PageModel
    {
        public void OnGet(bool success = false)
        {
            var uid = HttpContext.Session.GetInt32("UserId");
            if(uid == null)
            {
                Response.Redirect("/Account/Login");
                return;
            }
            ViewData["ShowSuccess"] = success;
        }
    }
}

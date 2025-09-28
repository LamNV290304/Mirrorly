using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Mirrorly.Models;
using Mirrorly.Services.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Mirrorly.Pages
{
    public abstract class BasePageModel : PageModel
    {
        protected readonly ProjectExeContext _context;
        protected readonly IVerificationServices? _verificationServices;
        protected readonly ITwoFactorServices? _twoFactorServices;

        protected BasePageModel(ProjectExeContext context,
            IVerificationServices? verificationServices = null,
            ITwoFactorServices? twoFactorServices = null)
        {
            _context = context;
            _verificationServices = verificationServices;
            _twoFactorServices = twoFactorServices;
        }

        // Current user properties
        protected int? CurrentUserId => HttpContext.Session.GetInt32("UserId");
        protected byte? CurrentUserRole => (byte?)HttpContext.Session.GetInt32("RoleId");
        protected string? CurrentUsername => HttpContext.Session.GetString("Username");
        protected string? CurrentUserEmail => HttpContext.Session.GetString("Email");

        // Role checks
        protected bool IsLoggedIn => CurrentUserId.HasValue;
        protected bool IsCustomer => CurrentUserRole == 1;
        protected bool IsMUA => CurrentUserRole == 2;
        protected bool IsAdmin => CurrentUserRole == 3;

        public override async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next)
        {
            // Load user status before page execution
            await LoadUserStatusAsync();

            // Execute the page handler
            await next.Invoke();
        }

        private async Task LoadUserStatusAsync()
        {
            if (!CurrentUserId.HasValue)
            {
                // Set default values for non-logged in users
                ViewData["IsVerified"] = false;
                ViewData["Has2FA"] = false;
                return;
            }

            try
            {
                // Load verification status for MUA
                if (IsMUA && _verificationServices != null)
                {
                    var isVerified = await _verificationServices.HasVerifiedIdentityAsync(CurrentUserId.Value);
                    ViewData["IsVerified"] = isVerified;
                }
                else
                {
                    ViewData["IsVerified"] = false;
                }

                // Load 2FA status for all users
                if (_twoFactorServices != null)
                {
                    var has2FA = await _twoFactorServices.Is2FAEnabledAsync(CurrentUserId.Value);
                    ViewData["Has2FA"] = has2FA;
                }
                else
                {
                    ViewData["Has2FA"] = false;
                }

                // Load additional user info if needed
                ViewData["CurrentUserId"] = CurrentUserId.Value;
                ViewData["CurrentUserRole"] = CurrentUserRole;
                ViewData["IsLoggedIn"] = IsLoggedIn;
                ViewData["IsMUA"] = IsMUA;
                ViewData["IsCustomer"] = IsCustomer;
                ViewData["IsAdmin"] = IsAdmin;
            }
            catch (Exception)
            {
                // Set default values on error
                ViewData["IsVerified"] = false;
                ViewData["Has2FA"] = false;
            }
        }

        // Helper methods for authorization
        protected bool RequireLogin()
        {
            if (!IsLoggedIn)
            {
                Response.Redirect("/Auth/Login");
                return false;
            }
            return true;
        }

        protected bool RequireRole(params byte[] allowedRoles)
        {
            if (!RequireLogin()) return false;

            if (!CurrentUserRole.HasValue || !allowedRoles.Contains(CurrentUserRole.Value))
            {
                Response.Redirect("/Error?statusCode=403");
                return false;
            }
            return true;
        }

        protected bool RequireMUA()
        {
            return RequireRole(2);
        }

        protected bool RequireCustomer()
        {
            return RequireRole(1);
        }

        protected bool RequireAdmin()
        {
            return RequireRole(3);
        }
    }
}

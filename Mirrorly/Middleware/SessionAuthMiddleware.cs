using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Mirrorly.Middleware
{
    public class SessionAuthMiddleware
    {
        private readonly RequestDelegate _next;

        public SessionAuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value?.ToLower();

            // Protected paths that require authentication
            var protectedPaths = new[]
            {
                "/profile/customerprofile",
                "/profile/muaprofile",
                "/mua/services",
                "/mua/portfolio",
                "/mua/bookings",
                "/mua/workinghours",
                "/customer/bookings",
                "/customer/favorites"
            };

            // MUA-only paths
            var muaPaths = new[]
            {
                "/profile/muaprofile",
                "/mua/services",
                "/mua/portfolio",
                "/mua/bookings",
                "/mua/workinghours"
            };

            // Customer-only paths
            var customerPaths = new[]
            {
                "/profile/customerprofile",
                "/customer/bookings",
                "/customer/favorites"
            };

            if (path != null && protectedPaths.Any(p => path.StartsWith(p)))
            {
                var userId = context.Session.GetInt32("UserId");
                var roleId = context.Session.GetInt32("RoleId");

                if (!userId.HasValue)
                {
                    context.Response.Redirect("/Auth/Login");
                    return;
                }

                // Check role-specific access
                if (muaPaths.Any(p => path.StartsWith(p)) && roleId != 2)
                {
                    context.Response.Redirect("/Auth/Login");
                    return;
                }

                if (customerPaths.Any(p => path.StartsWith(p)) && roleId != 1)
                {
                    context.Response.Redirect("/Auth/Login");
                    return;
                }
            }

            await _next(context);
        }
    }

    // Extension method to add middleware
    public static class SessionAuthMiddlewareExtensions
    {
        public static IApplicationBuilder UseSessionAuth(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<SessionAuthMiddleware>();
        }
    }
}

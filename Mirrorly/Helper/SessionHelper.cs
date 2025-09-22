using Microsoft.AspNetCore.Http;

namespace Mirrorly.Helpers
{
    public static class SessionHelper
    {
        public static bool IsLoggedIn(ISession session)
        {
            return session.GetInt32("UserId").HasValue;
        }

        public static bool IsMUA(ISession session)
        {
            var roleId = session.GetInt32("RoleId");
            return roleId == 2;
        }

        public static bool IsCustomer(ISession session)
        {
            var roleId = session.GetInt32("RoleId");
            return roleId == 1;
        }

        public static int? GetUserId(ISession session)
        {
            return session.GetInt32("UserId");
        }

        public static string? GetUsername(ISession session)
        {
            return session.GetString("Username");
        }

        public static string? GetEmail(ISession session)
        {
            return session.GetString("Email");
        }

        public static void SetUserSession(ISession session, int userId, string username, string email, string fullName, int roleId)
        {
            session.SetInt32("UserId", userId);
            session.SetString("Username", username);
            session.SetString("Email", email);
            session.SetString("FullName", fullName);
            session.SetInt32("RoleId", roleId);
        }

        public static void ClearSession(ISession session)
        {
            session.Clear();
        }
    }
}
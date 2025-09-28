using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Mirrorly.Services.Interfaces;
using Mirrorly.Models;

namespace Mirrorly.Pages.Admin.Users
{
    public class IndexModel : PageModel
    {
        private readonly IAdminServices _adminServices;

        public IndexModel(IAdminServices adminServices)
        {
            _adminServices = adminServices;
        }

        public List<User> Users { get; set; } = new();
        public int TotalUsers { get; set; }
        public int FilteredCount { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public string SearchTerm { get; set; } = "";
        public int? SelectedRole { get; set; }
        public string SelectedStatus { get; set; } = "";
        public string SortBy { get; set; } = "newest";
        public List<int> VerifiedMuaIds { get; set; } = new();

        public async Task OnGetAsync(int page = 1, string search = "", int? role = null,
            string status = "", string sort = "newest")
        {
            // Check admin permission
            var roleId = HttpContext.Session.GetInt32("RoleId");
            if (roleId != 3)
            {
                Response.Redirect("/Auth/Login");
                return;
            }

            CurrentPage = page;
            SearchTerm = search;
            SelectedRole = role;
            SelectedStatus = status;
            SortBy = sort;

            var filter = new UserFilterDto
            {
                SearchTerm = search,
                RoleId = role,
                Status = status,
                SortBy = sort,
                Page = page,
                PageSize = 20
            };

            var result = await _adminServices.GetFilteredUsersAsync(filter);
            Users = result.Users;
            TotalUsers = result.TotalUsers;
            FilteredCount = result.FilteredCount;
            TotalPages = (int)Math.Ceiling((double)FilteredCount / 20);

            // Get verified MUA IDs
            VerifiedMuaIds = await _adminServices.GetVerifiedMuaIdsAsync();
        }

        public async Task<IActionResult> OnPostToggleStatusAsync([FromBody] ToggleUserStatusDto request)
        {
            var roleId = HttpContext.Session.GetInt32("RoleId");
            if (roleId != 3) return Forbid();

            var success = await _adminServices.ToggleUserStatusAsync(request.UserId, request.IsActive);
            return new JsonResult(new { success });
        }

        public async Task<IActionResult> OnGetExportAsync()
        {
            var roleId = HttpContext.Session.GetInt32("RoleId");
            if (roleId != 3) return Forbid();

            // Export users to Excel
            var excelData = await _adminServices.ExportUsersAsync();
            return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "users_export.xlsx");
        }
    }
}
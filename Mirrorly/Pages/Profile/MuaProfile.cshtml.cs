using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Mirrorly.Services.Interfaces;
using Mirrorly.Models;
using System.ComponentModel.DataAnnotations;

namespace Mirrorly.Pages.Profile
{
    public class MuaProfileModel : BasePageModel
    {
        private readonly IProfileServices _profileServices;
        private readonly IAuthServices _authServices;

        public MuaProfileModel(IProfileServices profileServices, IAuthServices authServices,
            ProjectExeContext context, IVerificationServices verificationServices, ITwoFactorServices twoFactorServices)
            : base(context, verificationServices, twoFactorServices)
        {
            _profileServices = profileServices;
            _authServices = authServices;
        }

        [BindProperty]
        [Required(ErrorMessage = "Tên hiển thị là bắt buộc")]
        [StringLength(120, ErrorMessage = "Tên hiển thị không được quá 120 ký tự")]
        public string DisplayName { get; set; } = "";

        [BindProperty]
        public string? Bio { get; set; }

        [BindProperty]
        [StringLength(255, ErrorMessage = "Địa chỉ không được quá 255 ký tự")]
        public string? AddressLine { get; set; }

        [BindProperty]
        [Range(0, 50, ErrorMessage = "Số năm kinh nghiệm phải từ 0-50 năm")]
        public int? ExperienceYears { get; set; }

        [BindProperty]
        public bool ProfilePublic { get; set; }

        // Display Properties
        public string Email { get; set; } = "";
        public string Username { get; set; } = "";
        public List<Service> Services { get; set; } = new();
        public List<PortfolioItem> Portfolios { get; set; } = new();
        public List<WorkingHour> WorkingHours { get; set; } = new();
        public List<Category> Categories { get; set; } = new();

        [TempData]
        public string? SuccessMessage { get; set; }

        [TempData]
        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (!RequireMUA()) return Page();

            var user = await _authServices.GetUserById(CurrentUserId!.Value);
            if (user == null) return RedirectToPage("/Auth/Login");

            // Load basic info
            var muaProfile = await _profileServices.GetMuaProfile(CurrentUserId.Value);
            if (muaProfile != null)
            {
                DisplayName = muaProfile.DisplayName;
                Bio = muaProfile.Bio;
                AddressLine = muaProfile.AddressLine;
                ExperienceYears = muaProfile.ExperienceYears;
                ProfilePublic = muaProfile.ProfilePublic;
            }
            else
            {
                DisplayName = user.FullName ?? "";
                ProfilePublic = false;
            }

            Email = user.Email;
            Username = user.Username;

            // Load related data
            await LoadRelatedDataAsync(CurrentUserId.Value);

            return Page();
        }

        private async Task LoadRelatedDataAsync(int userId)
        {
            // Load services
            Services = await _context.Services
                .Include(s => s.Category)
                .Where(s => s.MuaId == userId)
                .OrderBy(s => s.Name)
                .ToListAsync();

            // Load portfolio
            Portfolios = await _context.PortfolioItems
                .Where(p => p.Muaid == userId)
                .OrderByDescending(p => p.CreatedAtUtc)
                .ToListAsync();

            // Load working hours
            WorkingHours = await _context.WorkingHours
                .Where(w => w.MuaId == userId)
                .OrderBy(w => w.DayOfWeek)
                .ToListAsync();

            // Load categories for dropdown
            Categories = await _context.Categories
                .OrderBy(c => c.CategoryName)
                .ToListAsync();
        }

        // Update Basic Information
        public async Task<IActionResult> OnPostUpdateBasicInfoAsync()
        {
            if (!RequireMUA()) return Page();

            if (string.IsNullOrWhiteSpace(DisplayName))
            {
                ModelState.AddModelError("DisplayName", "Tên hiển thị là bắt buộc");
            }

            if (!ModelState.IsValid)
            {
                await LoadRelatedDataAsync(CurrentUserId!.Value);
                return Page();
            }

            try
            {
                var muaProfile = await _profileServices.GetMuaProfile(CurrentUserId.Value);

                if (muaProfile == null)
                {
                    muaProfile = new Muaprofile
                    {
                        Muaid = CurrentUserId.Value,
                        DisplayName = DisplayName,
                        Bio = Bio,
                        AddressLine = AddressLine,
                        ExperienceYears = ExperienceYears,
                        ProfilePublic = ProfilePublic
                    };

                    await _profileServices.CreateMuaProfile(muaProfile);
                }
                else
                {
                    muaProfile.DisplayName = DisplayName;
                    muaProfile.Bio = Bio;
                    muaProfile.AddressLine = AddressLine;
                    muaProfile.ExperienceYears = ExperienceYears;
                    muaProfile.ProfilePublic = ProfilePublic;

                    await _profileServices.UpdateMuaProfile(muaProfile);
                }

                TempData["SuccessMessage"] = "Thông tin cơ bản đã được cập nhật thành công!";
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi cập nhật thông tin.";
            }

            return RedirectToPage();
        }

        // Add Service
        public async Task<IActionResult> OnPostAddServiceAsync(string serviceName, string description,
            decimal basePrice, int durationMin, int? categoryId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue) return RedirectToPage("/Auth/Login");

            if (string.IsNullOrWhiteSpace(serviceName) || basePrice <= 0)
            {
                TempData["ErrorMessage"] = "Tên dịch vụ và giá là bắt buộc!";
                return RedirectToPage();
            }

            try
            {
                var service = new Service
                {
                    MuaId = userId.Value,
                    Name = serviceName,
                    Description = description,
                    BasePrice = basePrice,
                    Currency = "VND",
                    DurationMin = durationMin > 0 ? durationMin : 90,
                    CategoryId = categoryId,
                    Active = true
                };

                _context.Services.Add(service);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Dịch vụ '{serviceName}' đã được thêm thành công!";
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi thêm dịch vụ.";
            }

            return RedirectToPage();
        }

        // Update Service
        public async Task<IActionResult> OnPostUpdateServiceAsync(long serviceId, string serviceName,
            string description, decimal basePrice, int durationMin, bool active)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue) return RedirectToPage("/Auth/Login");

            try
            {
                var service = await _context.Services
                    .FirstOrDefaultAsync(s => s.ServiceId == serviceId && s.MuaId == userId.Value);

                if (service == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy dịch vụ.";
                    return RedirectToPage();
                }

                service.Name = serviceName;
                service.Description = description;
                service.BasePrice = basePrice;
                service.DurationMin = durationMin;
                service.Active = active;

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Dịch vụ '{serviceName}' đã được cập nhật!";
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi cập nhật dịch vụ.";
            }

            return RedirectToPage();
        }

        // Delete Service
        public async Task<IActionResult> OnPostDeleteServiceAsync(long serviceId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue) return RedirectToPage("/Auth/Login");

            try
            {
                var service = await _context.Services
                    .FirstOrDefaultAsync(s => s.ServiceId == serviceId && s.MuaId == userId.Value);

                if (service == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy dịch vụ.";
                    return RedirectToPage();
                }

                // Check if service has bookings
                var hasBookings = await _context.BookingItems
                    .AnyAsync(bi => bi.ServiceId == serviceId);

                if (hasBookings)
                {
                    TempData["ErrorMessage"] = "Không thể xóa dịch vụ đã có booking. Bạn có thể tắt dịch vụ thay thế.";
                    return RedirectToPage();
                }

                _context.Services.Remove(service);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Dịch vụ đã được xóa thành công!";
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi xóa dịch vụ.";
            }

            return RedirectToPage();
        }

        // Add Portfolio
        public async Task<IActionResult> OnPostAddPortfolioAsync(string title, string description, string mediaUrl)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue) return RedirectToPage("/Auth/Login");

            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(mediaUrl))
            {
                TempData["ErrorMessage"] = "Tiêu đề và URL hình ảnh là bắt buộc!";
                return RedirectToPage();
            }

            // Validate URL
            if (!Uri.TryCreate(mediaUrl, UriKind.Absolute, out _))
            {
                TempData["ErrorMessage"] = "URL hình ảnh không hợp lệ!";
                return RedirectToPage();
            }

            try
            {
                var portfolio = new PortfolioItem
                {
                    Muaid = userId.Value,
                    Title = title,
                    Description = description,
                    MediaUrl = mediaUrl,
                    CreatedAtUtc = DateTime.UtcNow
                };

                _context.PortfolioItems.Add(portfolio);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Hình ảnh '{title}' đã được thêm vào portfolio!";
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi thêm hình ảnh.";
            }

            return RedirectToPage();
        }

        // Delete Portfolio
        public async Task<IActionResult> OnPostDeletePortfolioAsync(long itemId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue) return RedirectToPage("/Auth/Login");

            try
            {
                var portfolio = await _context.PortfolioItems
                    .FirstOrDefaultAsync(p => p.ItemId == itemId && p.Muaid == userId.Value);

                if (portfolio == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy hình ảnh.";
                    return RedirectToPage();
                }

                _context.PortfolioItems.Remove(portfolio);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Hình ảnh đã được xóa khỏi portfolio!";
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi xóa hình ảnh.";
            }

            return RedirectToPage();
        }

        // Update Working Hours
        public async Task<IActionResult> OnPostUpdateWorkingHoursAsync()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue) return RedirectToPage("/Auth/Login");

            try
            {
                // Remove existing working hours
                var existingHours = await _context.WorkingHours
                    .Where(w => w.MuaId == userId.Value)
                    .ToListAsync();

                _context.WorkingHours.RemoveRange(existingHours);

                // Add new working hours
                for (byte day = 1; day <= 7; day++)
                {
                    var isWorkingKey = $"IsWorking_{day}";
                    var startTimeKey = $"StartTime_{day}";
                    var endTimeKey = $"EndTime_{day}";

                    if (Request.Form.ContainsKey(isWorkingKey) &&
                        Request.Form[isWorkingKey].ToString().Contains("true"))
                    {
                        var startTimeStr = Request.Form[startTimeKey].ToString();
                        var endTimeStr = Request.Form[endTimeKey].ToString();

                        if (TimeOnly.TryParse(startTimeStr, out var startTime) &&
                            TimeOnly.TryParse(endTimeStr, out var endTime))
                        {
                            var workingHour = new WorkingHour
                            {
                                MuaId = userId.Value,
                                DayOfWeek = day,
                                StartTime = startTime,
                                EndTime = endTime
                            };

                            _context.WorkingHours.Add(workingHour);
                        }
                    }
                }

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Lịch làm việc đã được cập nhật thành công!";
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi cập nhật lịch làm việc.";
            }

            return RedirectToPage();
        }
    }
}
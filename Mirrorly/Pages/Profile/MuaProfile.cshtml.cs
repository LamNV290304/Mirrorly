using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Mirrorly.Models;
using Mirrorly.Services.Interfaces;
using RestSharp;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace Mirrorly.Pages.Profile
{
    public class MuaProfileModel : BasePageModel
    {
        private readonly IProfileServices _profileServices;
        private readonly IAuthServices _authServices;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;

        public MuaProfileModel(IProfileServices profileServices, IAuthServices authServices,
            ProjectExeContext context, IVerificationServices verificationServices, ITwoFactorServices twoFactorServices, IConfiguration configuration, IWebHostEnvironment env)
            : base(context, verificationServices, twoFactorServices)
        {
            _profileServices = profileServices;
            _authServices = authServices;
            _configuration = configuration;
            _env = env;
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

        [BindProperty]
        [Required(ErrorMessage = "Vui lòng chọn file ảnh")]
        public IFormFile? ImageFile { get; set; }

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
        public async Task<IActionResult> OnPostAddServiceAsync(
    string serviceName,
    string description,
    decimal basePrice,
    int durationMin,
    int? categoryId,
    IFormFile? imageFile)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
                return RedirectToPage("/Auth/Login");

            if (string.IsNullOrWhiteSpace(serviceName) || basePrice <= 0)
            {
                TempData["ErrorMessage"] = "Tên dịch vụ và giá là bắt buộc!";
                return RedirectToPage();
            }
            string? imageUrl = null;
            if (imageFile != null && imageFile.Length > 0)
            {
                var uploadDir = Path.Combine(_env.WebRootPath, "uploads", "services");
                Directory.CreateDirectory(uploadDir);
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(imageFile.FileName)}";
                var filePath = Path.Combine(uploadDir, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }
                imageUrl = $"/uploads/services/{fileName}";
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
                    Active = true,
                    ImageUrl = imageUrl
                };

                _context.Services.Add(service);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Dịch vụ '{serviceName}' đã được thêm thành công!";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi thêm dịch vụ.";
            }

            return RedirectToPage();
        }


        // Update Service
        public async Task<IActionResult> OnPostUpdateServiceAsync(long serviceId, string serviceName,
            string description, decimal basePrice, int durationMin, bool active, IFormFile? ImageFile)
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

                if (ImageFile != null && ImageFile.Length > 0)
                {
                    var uploadDir = Path.Combine(_env.WebRootPath, "uploads", "services");
                    Directory.CreateDirectory(uploadDir);
                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(ImageFile.FileName)}";
                    var filePath = Path.Combine(uploadDir, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(stream);
                    }
                    service.ImageUrl = $"/uploads/services/{fileName}";
                }
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Dịch vụ '{serviceName}' đã được cập nhật!";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
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
        public async Task<IActionResult> OnPostAddPortfolioAsync(string title, string description)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue) return RedirectToPage("/Auth/Login");

            if (string.IsNullOrWhiteSpace(title) || ImageFile == null)
            {
                TempData["ErrorMessage"] = "Tiêu đề và file ảnh là bắt buộc!";
                return RedirectToPage();
            }

            // Validate file
            if (!IsValidImageFile(ImageFile))
            {
                TempData["ErrorMessage"] = "File không phải ảnh hợp lệ (chỉ JPG, PNG, GIF)!";
                return RedirectToPage();
            }

            if (ImageFile.Length > 32 * 1024 * 1024) // 32MB
            {
                TempData["ErrorMessage"] = "File ảnh quá lớn (tối đa 32MB)!";
                return RedirectToPage();
            }

            string? mediaUrl = null;
            try
            {
                // Upload to ImgBB
                mediaUrl = await UploadToImgBBAsync(ImageFile);
                if (string.IsNullOrEmpty(mediaUrl))
                {
                    TempData["ErrorMessage"] = "Lỗi khi upload ảnh lên server!";
                    return RedirectToPage();
                }

                // Save to DB
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
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Có lỗi xảy ra khi thêm hình ảnh: {ex.Message}";
            }

            return RedirectToPage();
        }

        // Helper: Validate image file
        private bool IsValidImageFile(IFormFile file)
        {
            if (file.ContentType.StartsWith("image/"))
            {
                var allowedTypes = new[] { "image/jpeg", "image/png", "image/gif" };
                return allowedTypes.Contains(file.ContentType.ToLower());
            }
            return false;
        }

        // Helper: Upload to ImgBB
        private async Task<string?> UploadToImgBBAsync(IFormFile file)
        {
            var apiKey = _configuration["ImgBB:ApiKey"];
            if (string.IsNullOrEmpty(apiKey)) return null;

            var client = new RestClient("https://api.imgbb.com");
            var request = new RestRequest("/1/upload", Method.Post);

            // Add parameters
            request.AddParameter("key", apiKey);
            request.AddParameter("expiration", 0); // Không hết hạn

            // Đọc file thành byte array
            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                var fileBytes = memoryStream.ToArray();

                // Add file
                request.AddFile("image", fileBytes, file.FileName, file.ContentType);
            }

            var response = await client.ExecuteAsync(request);
            if (!response.IsSuccessful || response.Content == null) return null;

            // Parse JSON response
            var jsonDoc = JsonDocument.Parse(response.Content);
            var url = jsonDoc.RootElement.GetProperty("data").GetProperty("url").GetString();
            return url;
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
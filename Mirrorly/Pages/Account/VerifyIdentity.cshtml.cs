using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Mirrorly.Models;
using Mirrorly.Services.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Mirrorly.Pages.Account
{
    public class VerifyIdentityModel : PageModel
    {
        private readonly ProjectExeContext _context;
        private readonly IVerificationServices _verificationServices;

        public VerifyIdentityModel(ProjectExeContext context, IVerificationServices verificationServices)
        {
            _context = context;
            _verificationServices = verificationServices;
        }

        [BindProperty]
        [Required(ErrorMessage = "Họ tên là bắt buộc")]
        [StringLength(100, ErrorMessage = "Họ tên không được quá 100 ký tự")]
        public string FullName { get; set; } = "";

        [BindProperty]
        [Required(ErrorMessage = "Số CMND/CCCD là bắt buộc")]
        [RegularExpression(@"^\d{9}$|^\d{12}$", ErrorMessage = "Số CMND phải có 9 số hoặc CCCD phải có 12 số")]
        public string IdNumber { get; set; } = "";

        [BindProperty]
        [Required(ErrorMessage = "Ngày sinh là bắt buộc")]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Địa chỉ là bắt buộc")]
        [StringLength(500, ErrorMessage = "Địa chỉ không được quá 500 ký tự")]
        public string Address { get; set; } = "";

        public IdentityVerification? ExistingVerification { get; set; }

        [TempData]
        public string? SuccessMessage { get; set; }

        [TempData]
        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToPage("/Auth/Login");
            }

            // Load existing verification if any
            ExistingVerification = await _context.IdentityVerifications
                .FirstOrDefaultAsync(v => v.UserId == userId.Value);

            // Pre-populate form with user data if no existing verification
            if (ExistingVerification == null)
            {
                var user = await _context.Users.FindAsync(userId.Value);
                if (user != null)
                {
                    FullName = user.FullName ?? "";
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(IFormFile frontIdImage, IFormFile backIdImage, IFormFile? selfieImage)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToPage("/Auth/Login");
            }

            // Validate required files
            if (frontIdImage == null || backIdImage == null)
            {
                ModelState.AddModelError("", "Vui lòng upload ảnh mặt trước và mặt sau của CMND/CCCD");
            }

            if (!ModelState.IsValid)
            {
                await OnGetAsync();
                return Page();
            }

            try
            {
                // Check if user already has pending verification
                var existingVerification = await _context.IdentityVerifications
                    .FirstOrDefaultAsync(v => v.UserId == userId.Value && v.Status == 0);

                if (existingVerification != null)
                {
                    ErrorMessage = "Bạn đã có yêu cầu xác minh đang chờ xử lý.";
                    return RedirectToPage();
                }

                // Upload images and create verification request
                var verification = new IdentityVerification
                {
                    UserId = userId.Value,
                    FullName = FullName,
                    IdNumber = IdNumber,
                    DateOfBirth = DateOfBirth,
                    Address = Address,
                    Status = 0, // Pending
                    CreatedAt = DateTime.UtcNow
                };

                // Upload files (placeholder - implement actual file upload)
                verification.FrontIdImageUrl = await UploadFileAsync(frontIdImage, "front-id");
                verification.BackIdImageUrl = await UploadFileAsync(backIdImage, "back-id");

                if (selfieImage != null)
                {
                    verification.SelfieImageUrl = await UploadFileAsync(selfieImage, "selfie");
                }

                var success = await _verificationServices.CreateVerificationRequestAsync(verification);

                if (success)
                {
                    SuccessMessage = "Yêu cầu xác minh đã được gửi thành công! Chúng tôi sẽ xem xét trong vòng 24-48 giờ.";
                    return RedirectToPage();
                }
                else
                {
                    ErrorMessage = "Có lỗi xảy ra khi gửi yêu cầu. Vui lòng thử lại.";
                }
            }
            catch (Exception)
            {
                ErrorMessage = "Có lỗi hệ thống. Vui lòng thử lại sau.";
            }

            await OnGetAsync();
            return Page();
        }

        private async Task<string> UploadFileAsync(IFormFile file, string prefix)
        {
            // TODO: Implement actual file upload to cloud storage or local storage
            // For now, return a placeholder URL
            var fileName = $"{prefix}_{DateTime.UtcNow.Ticks}_{file.FileName}";
            return $"/uploads/verifications/{fileName}";
        }
    }
}
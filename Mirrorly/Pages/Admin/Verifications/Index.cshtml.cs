// Pages/Admin/Verifications/Index.cshtml.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Mirrorly.Models;
using Mirrorly.Services.Interfaces;

namespace Mirrorly.Pages.Admin.Verifications
{
    public class IndexModel : PageModel
    {
        private readonly IVerificationServices _verificationServices;
        private readonly ProjectExeContext _context;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(
            IVerificationServices verificationServices,
            ProjectExeContext context,
            ILogger<IndexModel> logger)
        {
            _verificationServices = verificationServices;
            _context = context;
            _logger = logger;
        }

        // Properties for View binding
        public List<IdentityVerification> Verifications { get; set; } = new();
        public string CurrentFilter { get; set; } = "all";
        public int TotalPending { get; set; }
        public int TotalApproved { get; set; }
        public int TotalRejected { get; set; }

        // Admin information
        public string? AdminName { get; set; }
        public int? AdminId { get; set; }

        // Pagination
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 10;
        public bool HasPrevious => CurrentPage > 1;
        public bool HasNext => CurrentPage < TotalPages;

        // Search functionality
        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        // Sorting
        [BindProperty(SupportsGet = true)]
        public string SortBy { get; set; } = "date";

        [BindProperty(SupportsGet = true)]
        public string SortOrder { get; set; } = "desc";

        [TempData]
        public string? SuccessMessage { get; set; }

        [TempData]
        public string? ErrorMessage { get; set; }

        [TempData]
        public string? InfoMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(
            string filter = "all",
            int page = 1,
            string? search = null,
            string sortBy = "date",
            string sortOrder = "desc")
        {
            // Check admin authentication
            var roleId = HttpContext.Session.GetInt32("RoleId");
            var userId = HttpContext.Session.GetInt32("UserId");

            if (roleId != 3 || !userId.HasValue)
            {
                ErrorMessage = "Bạn không có quyền truy cập vào trang này";
                return RedirectToPage("/Auth/Login");
            }

            // Set admin info
            AdminId = userId.Value;
            AdminName = HttpContext.Session.GetString("Username") ?? "Admin";

            // Set parameters
            CurrentFilter = filter;
            CurrentPage = Math.Max(1, page);
            SearchTerm = search;
            SortBy = sortBy;
            SortOrder = sortOrder;

            try
            {
                // Load statistics
                await LoadStatisticsAsync();

                // Load verifications with pagination and filtering
                await LoadVerificationsAsync(filter, page, search, sortBy, sortOrder);

                // Log admin activity
                _logger.LogInformation("Admin {AdminId} accessed verification management page with filter: {Filter}",
                    AdminId, filter);

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading verification management page for admin {AdminId}", AdminId);
                ErrorMessage = "Có lỗi xảy ra khi tải dữ liệu. Vui lòng thử lại.";
                return Page();
            }
        }

        public async Task<IActionResult> OnPostProcessVerificationAsync(
            long verificationId,
            bool approved,
            string? adminNotes)
        {
            var roleId = HttpContext.Session.GetInt32("RoleId");
            var adminId = HttpContext.Session.GetInt32("UserId");

            if (roleId != 3 || !adminId.HasValue)
            {
                ErrorMessage = "Bạn không có quyền thực hiện thao tác này";
                return RedirectToPage("/Auth/Login");
            }

            // Validate input
            if (verificationId <= 0)
            {
                ErrorMessage = "ID xác minh không hợp lệ";
                return RedirectToPage(new { filter = Request.Query["filter"].ToString() });
            }

            try
            {
                // Get verification details before processing
                var verification = await _verificationServices.GetVerificationByIdAsync(verificationId);

                if (verification == null)
                {
                    ErrorMessage = "Không tìm thấy yêu cầu xác minh";
                    return RedirectToPage(new { filter = Request.Query["filter"].ToString() });
                }

                if (verification.Status != 0)
                {
                    ErrorMessage = "Yêu cầu này đã được xử lý trước đó";
                    return RedirectToPage(new { filter = Request.Query["filter"].ToString() });
                }

                // Process verification
                var success = await _verificationServices.ProcessVerificationAsync(
                    verificationId, approved, adminNotes, adminId.Value);

                if (success)
                {
                    var actionText = approved ? "duyệt" : "từ chối";
                    SuccessMessage = $"✅ Đã {actionText} yêu cầu xác minh cho {verification.User?.FullName}";

                    // Log admin action
                    _logger.LogInformation(
                        "Admin {AdminId} {Action} verification {VerificationId} for user {UserId} with notes: {Notes}",
                        adminId.Value,
                        approved ? "approved" : "rejected",
                        verificationId,
                        verification.UserId,
                        adminNotes ?? "No notes");

                    // Send notification email (async, don't wait)
                    _ = Task.Run(async () => await SendVerificationResultEmailAsync(verification, approved, adminNotes));
                }
                else
                {
                    ErrorMessage = "❌ Có lỗi xảy ra khi xử lý yêu cầu xác minh";
                    _logger.LogError("Failed to process verification {VerificationId} by admin {AdminId}",
                        verificationId, adminId.Value);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing verification {VerificationId} by admin {AdminId}",
                    verificationId, adminId);
                ErrorMessage = "❌ Có lỗi hệ thống khi xử lý yêu cầu";
            }

            return RedirectToPage(new
            {
                filter = Request.Query["filter"].ToString(),
                page = Request.Query["page"].ToString(),
                search = Request.Query["search"].ToString(),
                sortBy = Request.Query["sortBy"].ToString(),
                sortOrder = Request.Query["sortOrder"].ToString()
            });
        }

        // AJAX endpoint for quick stats refresh
        public async Task<IActionResult> OnGetStatsAsync()
        {
            var roleId = HttpContext.Session.GetInt32("RoleId");
            if (roleId != 3)
            {
                return new JsonResult(new { error = "Unauthorized" }) { StatusCode = 401 };
            }

            try
            {
                await LoadStatisticsAsync();
                return new JsonResult(new
                {
                    pending = TotalPending,
                    approved = TotalApproved,
                    rejected = TotalRejected,
                    total = TotalPending + TotalApproved + TotalRejected
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading stats for admin dashboard");
                return new JsonResult(new { error = "Failed to load stats" }) { StatusCode = 500 };
            }
        }

        // Bulk action endpoint
        public async Task<IActionResult> OnPostBulkActionAsync(
            string action,
            long[] verificationIds,
            string? bulkNotes)
        {
            var roleId = HttpContext.Session.GetInt32("RoleId");
            var adminId = HttpContext.Session.GetInt32("UserId");

            if (roleId != 3 || !adminId.HasValue)
            {
                return new JsonResult(new { error = "Unauthorized" }) { StatusCode = 401 };
            }

            if (verificationIds == null || !verificationIds.Any())
            {
                return new JsonResult(new { error = "Không có mục nào được chọn" }) { StatusCode = 400 };
            }

            try
            {
                var processed = 0;
                var errors = 0;

                foreach (var id in verificationIds)
                {
                    var success = action.ToLower() switch
                    {
                        "approve" => await _verificationServices.ProcessVerificationAsync(id, true, bulkNotes, adminId.Value),
                        "reject" => await _verificationServices.ProcessVerificationAsync(id, false, bulkNotes, adminId.Value),
                        _ => false
                    };

                    if (success) processed++;
                    else errors++;
                }

                _logger.LogInformation("Admin {AdminId} performed bulk {Action} on {Count} verifications",
                    adminId.Value, action, verificationIds.Length);

                return new JsonResult(new
                {
                    success = true,
                    processed = processed,
                    errors = errors,
                    message = $"Đã xử lý {processed} yêu cầu thành công" +
                             (errors > 0 ? $", {errors} yêu cầu thất bại" : "")
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in bulk action {Action} by admin {AdminId}", action, adminId);
                return new JsonResult(new { error = "Có lỗi xảy ra khi xử lý hàng loạt" }) { StatusCode = 500 };
            }
        }

        private async Task LoadStatisticsAsync()
        {
            var stats = await _context.IdentityVerifications
                .GroupBy(v => v.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToListAsync();

            TotalPending = stats.FirstOrDefault(s => s.Status == 0)?.Count ?? 0;
            TotalApproved = stats.FirstOrDefault(s => s.Status == 1)?.Count ?? 0;
            TotalRejected = stats.FirstOrDefault(s => s.Status == 2)?.Count ?? 0;
        }

        private async Task LoadVerificationsAsync(
            string filter,
            int page,
            string? search,
            string sortBy,
            string sortOrder)
        {
            var query = _context.IdentityVerifications
                .Include(v => v.User)
                .Include(v => v.ProcessedByAdmin)
                .AsQueryable();

            // Apply status filter
            query = filter switch
            {
                "pending" => query.Where(v => v.Status == 0),
                "approved" => query.Where(v => v.Status == 1),
                "rejected" => query.Where(v => v.Status == 2),
                _ => query
            };

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchLower = search.ToLower();
                query = query.Where(v =>
                    v.FullName.ToLower().Contains(searchLower) ||
                    v.IdNumber.Contains(search) ||
                    v.User.Email.ToLower().Contains(searchLower) ||
                    v.User.FullName.ToLower().Contains(searchLower));
            }

            // Apply sorting
            query = sortBy.ToLower() switch
            {
                "name" => sortOrder == "asc"
                    ? query.OrderBy(v => v.FullName)
                    : query.OrderByDescending(v => v.FullName),
                "email" => sortOrder == "asc"
                    ? query.OrderBy(v => v.User.Email)
                    : query.OrderByDescending(v => v.User.Email),
                "status" => sortOrder == "asc"
                    ? query.OrderBy(v => v.Status)
                    : query.OrderByDescending(v => v.Status),
                _ => sortOrder == "asc"
                    ? query.OrderBy(v => v.CreatedAt)
                    : query.OrderByDescending(v => v.CreatedAt)
            };

            // Calculate pagination
            var totalCount = await query.CountAsync();
            TotalPages = (int)Math.Ceiling(totalCount / (double)PageSize);
            CurrentPage = Math.Min(CurrentPage, Math.Max(1, TotalPages));

            // Get paginated results
            Verifications = await query
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();
        }

        private async Task SendVerificationResultEmailAsync(
            IdentityVerification verification,
            bool approved,
            string? notes)
        {
            if (verification?.User?.Email == null) return;

            try
            {
                // TODO: Implement with your email service
                // var emailService = HttpContext.RequestServices.GetService<IEmailService>();
                // if (emailService != null)
                // {
                //     var subject = approved ? "✅ Xác minh danh tính thành công - Mirrorly" : "❌ Yêu cầu xác minh bị từ chối - Mirrorly";
                //     var body = BuildEmailTemplate(verification, approved, notes);
                //     await emailService.SendEmailAsync(verification.User.Email, subject, body);
                // }

                _logger.LogInformation("Email notification sent to user {UserId} for verification result: {Result}",
                    verification.UserId, approved ? "approved" : "rejected");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email notification to user {UserId}", verification.UserId);
                // Don't throw - email failure shouldn't affect verification processing
            }
        }

        private string BuildEmailTemplate(IdentityVerification verification, bool approved, string? notes)
        {
            var statusText = approved ? "được chấp nhận" : "bị từ chối";
            var statusIcon = approved ? "✅" : "❌";
            var statusColor = approved ? "#10b981" : "#ef4444";
            var nextSteps = approved ?
                "Bây giờ bạn có thể sử dụng đầy đủ các tính năng dành cho MUA đã xác minh trên Mirrorly." :
                "Vui lòng kiểm tra lại thông tin và tài liệu, sau đó gửi lại yêu cầu xác minh.";

            var template = $@"
        <!DOCTYPE html>
        <html>
        <head>
            <meta charset='utf-8'>
            <meta name='viewport' content='width=device-width, initial-scale=1.0'>
            <title>Kết quả xác minh danh tính - Mirrorly</title>
        </head>
        <body style='margin: 0; padding: 0; background-color: #f3f4f6; font-family: Arial, sans-serif;'>
            <div style='max-width: 600px; margin: 0 auto; background-color: white;'>
                
                <!-- Header -->
                <div style='background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); padding: 30px 20px; text-align: center; color: white;'>
                    <h1 style='margin: 0; font-size: 24px; font-weight: bold;'>
                        {statusIcon} Kết quả xác minh danh tính
                    </h1>
                    <p style='margin: 10px 0 0 0; opacity: 0.9;'>Mirrorly - Nền tảng makeup artist</p>
                </div>
                
                <!-- Main Content -->
                <div style='padding: 30px 20px;'>
                    <p style='font-size: 16px; color: #374151; margin: 0 0 20px 0;'>
                        Chào <strong>{verification.User.FullName}</strong>,
                    </p>
                    
                    <div style='background: {statusColor}; color: white; padding: 20px; border-radius: 12px; text-align: center; margin: 20px 0;'>
                        <h2 style='margin: 0 0 10px 0; font-size: 20px;'>
                            {statusIcon} Yêu cầu xác minh đã {statusText}
                        </h2>
                        <p style='margin: 0; opacity: 0.9;'>{nextSteps}</p>
                    </div>
                    
                    <!-- Verification Details -->
                    <div style='background: #f8fafc; padding: 20px; border-radius: 12px; margin: 25px 0; border: 1px solid #e2e8f0;'>
                        <h3 style='margin: 0 0 15px 0; color: #374151; font-size: 16px;'>📄 Chi tiết xác minh:</h3>
                        <div style='display: grid; gap: 10px;'>
                            <div style='display: flex; justify-content: space-between; padding: 8px 0; border-bottom: 1px solid #e2e8f0;'>
                                <span style='color: #6b7280; font-weight: 500;'>Họ tên:</span>
                                <span style='color: #374151; font-weight: 600;'>{verification.FullName}</span>
                            </div>
                            <div style='display: flex; justify-content: space-between; padding: 8px 0; border-bottom: 1px solid #e2e8f0;'>
                                <span style='color: #6b7280; font-weight: 500;'>Số CMND/CCCD:</span>
                                <span style='color: #374151; font-weight: 600;'>{verification.IdNumber}</span>
                            </div>
                            <div style='display: flex; justify-content: space-between; padding: 8px 0; border-bottom: 1px solid #e2e8f0;'>
                                <span style='color: #6b7280; font-weight: 500;'>Ngày gửi:</span>
                                <span style='color: #374151; font-weight: 600;'>{verification.CreatedAt:dd/MM/yyyy HH:mm}</span>
                            </div>
                            <div style='display: flex; justify-content: space-between; padding: 8px 0;'>
                                <span style='color: #6b7280; font-weight: 500;'>Ngày xử lý:</span>
                                <span style='color: #374151; font-weight: 600;'>{DateTime.UtcNow:dd/MM/yyyy HH:mm}</span>
                            </div>
                        </div>
                    </div>";

            // Add admin notes if provided
            if (!string.IsNullOrEmpty(notes))
            {
                template += $@"
                    <!-- Admin Notes -->
                    <div style='background: #fffbeb; padding: 20px; border-radius: 12px; margin: 25px 0; border: 1px solid #fcd34d;'>
                        <h3 style='margin: 0 0 10px 0; color: #92400e; font-size: 16px;'>📝 Ghi chú từ admin:</h3>
                        <p style='margin: 0; color: #92400e; line-height: 1.6;'>{notes}</p>
                    </div>";
            }

            // Add next steps based on approval status
            if (approved)
            {
                template += $@"
                    <!-- Success Next Steps -->
                    <div style='background: #dcfce7; padding: 20px; border-radius: 12px; margin: 25px 0; border: 1px solid #86efac;'>
                        <h3 style='margin: 0 0 15px 0; color: #166534; font-size: 16px;'>🎉 Chúc mừng! Tài khoản của bạn đã được xác minh</h3>
                        <div style='color: #166534; line-height: 1.6;'>
                            <p style='margin: 0 0 10px 0;'>Bây giờ bạn có thể:</p>
                            <ul style='margin: 10px 0; padding-left: 20px;'>
                                <li>Tạo và quản lý các dịch vụ makeup</li>
                                <li>Nhận booking từ khách hàng</li>
                                <li>Hiển thị portfolio công khai</li>
                                <li>Tham gia các chương trình khuyến mãi</li>
                                <li>Nhận huy hiệu ""Đã xác minh"" trên hồ sơ</li>
                            </ul>
                        </div>
                    </div>";
            }
            else
            {
                template += $@"
                    <!-- Rejection Next Steps -->
                    <div style='background: #fee2e2; padding: 20px; border-radius: 12px; margin: 25px 0; border: 1px solid #fca5a5;'>
                        <h3 style='margin: 0 0 15px 0; color: #991b1b; font-size: 16px;'>📋 Để hoàn tất xác minh, bạn cần:</h3>
                        <div style='color: #991b1b; line-height: 1.6;'>
                            <ul style='margin: 0; padding-left: 20px;'>
                                <li>Kiểm tra lại thông tin cá nhân cho chính xác</li>
                                <li>Đảm bảo hình ảnh CMND/CCCD rõ nét, không bị mờ</li>
                                <li>Ảnh selfie cần rõ mặt và rõ thông tin trên giấy tờ</li>
                                <li>Thông tin trên form phải khớp với giấy tờ</li>
                            </ul>
                            <p style='margin: 15px 0 0 0;'>
                                <a href='https://mirrorly.com/verification' 
                                   style='background: #ef4444; color: white; padding: 12px 20px; 
                                          text-decoration: none; border-radius: 8px; font-weight: 600;
                                          display: inline-block; margin-top: 10px;'>
                                    🔄 Gửi lại yêu cầu xác minh
                                </a>
                            </p>
                        </div>
                    </div>";
            }

            // Footer
            template += $@"
                    <!-- Support Information -->
                    <div style='background: #f1f5f9; padding: 20px; border-radius: 12px; margin: 25px 0 0 0; border: 1px solid #cbd5e1;'>
                        <h3 style='margin: 0 0 10px 0; color: #475569; font-size: 16px;'>💬 Cần hỗ trợ?</h3>
                        <p style='margin: 0 0 10px 0; color: #64748b; line-height: 1.6;'>
                            Nếu bạn có thắc mắc về quá trình xác minh, vui lòng liên hệ:
                        </p>
                        <div style='color: #64748b;'>
                            <p style='margin: 5px 0;'>📧 Email: support@mirrorly.com</p>
                            <p style='margin: 5px 0;'>📱 Hotline: 1900-MIRROR (1900-647767)</p>
                            <p style='margin: 5px 0;'>🕒 Thời gian hỗ trợ: 8:00 - 22:00 hàng ngày</p>
                        </div>
                    </div>
                </div>
                
                <!-- Footer -->
                <div style='background: #1e293b; color: #e2e8f0; padding: 25px 20px; text-align: center;'>
                    <div style='margin-bottom: 15px;'>
                        <h2 style='margin: 0; font-size: 20px; color: #f1f5f9;'>MIRRORLY</h2>
                        <p style='margin: 5px 0 0 0; opacity: 0.8; font-size: 14px;'>
                            Nền tảng kết nối makeup artist chuyên nghiệp
                        </p>
                    </div>
                    
                    <div style='border-top: 1px solid #374151; padding-top: 15px; margin-top: 15px;'>
                        <p style='margin: 0; font-size: 12px; opacity: 0.7;'>
                            Email này được gửi tự động từ hệ thống Mirrorly.<br>
                            Vui lòng không trả lời trực tiếp email này.
                        </p>
                        <p style='margin: 10px 0 0 0; font-size: 12px; opacity: 0.7;'>
                            © 2025 Mirrorly. All rights reserved.
                        </p>
                    </div>
                </div>
            </div>
        </body>
        </html>";

            return template;
        }
    }
}
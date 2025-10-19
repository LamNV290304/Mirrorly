using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Mirrorly.Models;
using Mirrorly.Services;
using Mirrorly.Services.Interfaces;

namespace Mirrorly.Pages.Books
{
    public class BookingModel : PageModel
    {
        private readonly IBookingService _bookingService;
        private readonly IAuthServices _authServices;
        private readonly IProfileServices _profileServices;
        private readonly ISeServices _seServices;
        private readonly IEmailService _emailService;

        public BookingModel(IBookingService bookingService, IAuthServices authServices, IProfileServices profileServices, ISeServices seServices, IEmailService emailService)
        {
            _bookingService = bookingService;
            _authServices = authServices;
            _profileServices = profileServices;
            _seServices = seServices;
            _emailService = emailService;
        }

        [TempData]
        public string? StatusMessage { get; set; }
        public User? LoggedUser { get; set; }
        public User? MuaUser { get; set; }
        public Service? Services { get; set; }
        public Muaprofile? MuaProfile { get; set; }

        public async Task<IActionResult> OnGet(int serviceId)
        {
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            if (userId == 0)
            {
                return RedirectToPage("/Auth/Login");
            }

            // Lấy service theo id truyền sang
            Services = await _seServices.GetServiceByIdAsync(serviceId);
            if (Services == null)
            {
                StatusMessage = "⚠️ Dịch vụ không tồn tại.";
                return Page();
            }

            // Gán giá trị mặc định cho BookingRequest để form tự hiển thị
            bookingRequest = new BookingRequest
            {
                Service = Services.ServiceId
            };

            LoggedUser = await _authServices.GetUserById(userId);
            MuaProfile = await _profileServices.GetMuaProfile(Services.MuaId);
            MuaUser = await _authServices.GetUserById(Services.MuaId);

            StatusMessage = "";
            return Page();
        }


        [BindProperty]
        public BookingRequest bookingRequest { get; set; } = new();


        public async Task<IActionResult> OnPostAsync()
        {
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;

            if (userId == 0)
            {
                return RedirectToPage("/Auth/Login");
            }

            LoggedUser = await _authServices.GetUserById(userId);

            if (!ModelState.IsValid)
            {
                StatusMessage = "❌ Dữ liệu không hợp lệ, vui lòng kiểm tra lại.";
                MuaProfile = await _profileServices.GetMuaProfile(bookingRequest.Muaid);
                return Page();
            }

            try
            {
                Mirrorly.Models.Booking booking = new Mirrorly.Models.Booking
                {
                    CustomerId = HttpContext.Session.GetInt32("UserId") ?? 0,
                    MuaId = bookingRequest.Muaid,
                    ScheduledStart = bookingRequest.Date,
                    TimeM = bookingRequest.Time,
                    AddressLine = bookingRequest.Address,
                    ServiceId = bookingRequest.Service,
                    Notes = bookingRequest.Notes,
                    Status = 2
                };

                _bookingService.AddBooking(booking);
                var service = await _seServices.GetServiceByIdAsync((int)bookingRequest.Service);
                var mua = await _authServices.GetUserById(bookingRequest.Muaid);

                string subject = $"[Mirrorly] Xác nhận đặt lịch Makeup - {service?.Name}";
                string htmlContent = $@"
        <h2>Tên khách hàng: {bookingRequest.Name},</h2>
        <p>Đã đặt lịch make up <b>Mirrorly</b>.</p>
        <p><b>Thông tin đặt lịch:</b></p>
        <ul>
            <li>💄 <b>Chuyên viên:</b> {mua?.FullName}</li>
            <li>📅 <b>Ngày:</b> {bookingRequest.Date:dd/MM/yyyy}</li>
            <li>⏰ <b>Giờ:</b> {bookingRequest.Time}</li>
            <li>📍 <b>Địa chỉ:</b> {bookingRequest.Address}</li>
            <li>💰 <b>Dịch vụ:</b> {service?.Name} - {service?.BasePrice:N0} đ</li>
            <li>📝 <b>Ghi chú:</b> {bookingRequest.Notes}</li>
        </ul>
        <p>Cảm ơn bạn đã tin tưởng <b>Mirrorly</b> 💕<br/>Hẹn gặp bạn trong buổi makeup sắp tới!</p>
        <hr/>
        <p style='font-size:12px;color:gray;'>Mirrorly Booking System - DVAT</p>
    ";

                // 🔹 Gửi email xác nhận
                await _emailService.SendEmailAsync(
                    "mirrorly.love@gmail.com", // 🔸 chỗ này thay bằng email thực của user nếu có
                    subject,
                    htmlContent);

                // 🔹 Gửi email cho MUA
                if (!string.IsNullOrEmpty(mua?.Email))
                {
                    await _emailService.SendEmailAsync(
                        mua.Email,
                        $"[Mirrorly] Bạn có lịch mới với {bookingRequest.Name}",
                        htmlContent);
                }
                //  StatusMessage = "✅ Đặt lịch thành công!";
                return RedirectToPage("/Payment/QR", new { bookingId = booking.BookingId, success = true });
            }
            catch (Exception)
            {
                StatusMessage = "⚠️ Có lỗi xảy ra, vui lòng thử lại.";
            }

            LoggedUser = await _authServices.GetUserById(LoggedUser.UserId);
            MuaProfile = await _profileServices.GetMuaProfile(bookingRequest.Muaid);
            MuaUser = await _authServices.GetUserById(bookingRequest.Muaid);

            return Page();
        }


        public class BookingRequest
        {
            public int Muaid { get; set; }
            public string Name { get; set; } = string.Empty;
            public string Phone { get; set; } = string.Empty;
            public DateTime Date { get; set; }
            public TimeSpan Time { get; set; }
            public string Address { get; set; } = string.Empty;
            public long Service { get; set; }
            public string? Notes { get; set; }
        }
    }
}

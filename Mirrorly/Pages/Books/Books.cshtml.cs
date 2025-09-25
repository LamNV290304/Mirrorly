using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Mirrorly.Models;
using Mirrorly.Services.Interfaces;

namespace Mirrorly.Pages.Books
{
    public class BookingModel : PageModel
    {
        private readonly IBookingService _bookingService;
        private readonly IAuthServices _authServices;
        private readonly IProfileServices _profileServices;

        public BookingModel(IBookingService bookingService, IAuthServices authServices, IProfileServices profileServices)
        {
            _bookingService = bookingService;
            _authServices = authServices;
            _profileServices = profileServices;
        }

        [TempData]
        public string? StatusMessage { get; set; }
        public User? LoggedUser { get; set; }
        public Muaprofile? MuaProfile { get; set; }

        public async Task<IActionResult> OnGet(int muaId)
        {
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;

            if (userId == 0)
            {
                return RedirectToPage("/Auth/Login");
            }

            LoggedUser = await _authServices.GetUserById(userId);

            MuaProfile = await _profileServices.GetMuaProfile(muaId);
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
                Booking booking = new Booking
                {
                    CustomerId = HttpContext.Session.GetInt32("UserId") ?? 0,
                    MuaId = bookingRequest.Muaid,
                    ScheduledStart = bookingRequest.Date,
                    TimeM = bookingRequest.Time,
                    AddressLine = bookingRequest.Address,
                    ServiceId = bookingRequest.Service,
                    Notes = bookingRequest.Notes,
                    Status = 3
                };

                _bookingService.AddBooking(booking);
                StatusMessage = "✅ Đặt lịch thành công!";
            }
            catch (Exception)
            {
                StatusMessage = "⚠️ Có lỗi xảy ra, vui lòng thử lại.";
            }

            LoggedUser = await _authServices.GetUserById(LoggedUser.UserId);
            MuaProfile = await _profileServices.GetMuaProfile(bookingRequest.Muaid);

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
            public int Service { get; set; }
            public string? Notes { get; set; }
        }
    }
}

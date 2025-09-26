using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Mirrorly.Services.Interfaces;

namespace Mirrorly.Pages.Books
{
    public class BookingStatusModel : PageModel
    {
        private readonly IBookingService _bookingService;
        public BookingStatusModel(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        public List<Models.Booking> Books;

        public async Task<IActionResult> OnGet()
        {
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;

            if (userId == 0)
            {
                return RedirectToPage("/Auth/Login");
            }

            Books = _bookingService.GetBookingsByCustomerId(userId);

            return Page();
        }

        [TempData]
        public string? Message { get; set; }

        [TempData]
        public string? Error { get; set; }

        public IActionResult OnPostCancel(int bookingId, int status)
        {
            try
            {
                _bookingService.ChangeBookingStatus(bookingId, 3);

                Message = $"Booking #{bookingId} đã bị hủy. Bạn đã chấp nhận mất tiền cọc.";
            }
            catch (Exception ex)
            {
                Error = $"Không thể hủy Booking #{bookingId}. Chi tiết: {ex.Message}";
            }

            return RedirectToPage();
        }
    }
}

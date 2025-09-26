using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Mirrorly.Models;
using Mirrorly.Services.Interfaces;

namespace Mirrorly.Pages.Books
{
    public class ArtistBookingsModel : PageModel
    {
        private readonly IBookingService _bookingService;

        public ArtistBookingsModel(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        public List<Booking> Books { get; set; } = new();

        [TempData] public string? Message { get; set; }
        [TempData] public string? Error { get; set; }

        public IActionResult OnGet()
        {
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            if (userId == 0)
            {
                return RedirectToPage("/Auth/Login");
            }

            Books = _bookingService.GetBookingsByMuaId(userId);
            return Page();
        }

        public IActionResult OnPost(int bookingId, int status)
        {
            try
            {
                _bookingService.ChangeBookingStatus(bookingId, status);
                Message = $"Booking #{bookingId} thay đổi trạng thái thành công ✅";
            }
            catch (Exception ex)
            {
                Error = $"Không thể cập nhật booking #{bookingId}. Chi tiết: {ex.Message}";
            }

            // redirect lại OnGet để load danh sách
            return RedirectToPage();
        }
    }
}

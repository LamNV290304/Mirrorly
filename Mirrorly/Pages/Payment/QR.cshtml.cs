using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Mirrorly.Models;
using Mirrorly.Services.Interfaces;
using System.Threading.Tasks;

namespace Mirrorly.Pages.Payment
{
    public class QRModel : PageModel
    {
        private readonly IBookingService _bookingService;

        public QRModel(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        public Models.Booking? Booking { get; set; }

        public async Task<IActionResult> OnGetAsync(int bookingId, bool success = false)
        {
            Booking =  _bookingService.GetBookingByBookingId(bookingId);

            if (Booking == null)
            {
                return NotFound();
            }

            ViewData["ShowSuccess"] = success;
            return Page();
        }
    }
}

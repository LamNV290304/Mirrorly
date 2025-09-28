using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Mirrorly.Services.Interfaces;

public class BookingDetailModel : PageModel
{
    private readonly IBookingService _bookingService;
    public BookingDetailModel(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    public BookingDetailViewModel? BookingDetail { get; set; }

    public IActionResult OnGet(long id)
    {
        BookingDetail = _bookingService.GetBookingDetail(id);
        if (BookingDetail == null)
            return NotFound();

        return Page();
    }
}


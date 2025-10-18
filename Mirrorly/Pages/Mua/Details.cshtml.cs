using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Mirrorly.Models;
using Mirrorly.Services;
using Mirrorly.Services.Interfaces;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Mirrorly.Pages.Mua
{
    public class DetailsModel : PageModel
    {
        private readonly IReviewServices _review;
        private readonly ISeServices _service;
        private readonly IPortfoServices _portfo;
        private readonly IWorkingHoursServices _workingHours;
        private readonly IMuaServices _muaServices;
        private readonly IBookingService _bookingService;
        private readonly IBookingService _booking;

        public DetailsModel(IReviewServices review, ISeServices service, IPortfoServices portfo, IWorkingHoursServices workingHours, IMuaServices muaServices, IBookingService bookingService)
        {
            _review = review;
            _service = service;
            _portfo = portfo;
            _workingHours = workingHours;
            _muaServices = muaServices;
            _bookingService = bookingService;
        }
        [BindProperty]
        public Service? Service { get; set; }
        public List<Review> Reviews { get; set; }
        public List<Service> Services { get; set; }
        public List<PortfolioItem> Portfolios { get; set; }
        public List<WorkingHour> WorkingHours { get; set; }
        public Muaprofile? MuaProfile { get; set; }
        [BindProperty]
        public Review NewReview { get; set; }
        public async Task OnGet(int id)
        {
            Service = await _service.GetServiceByIdAsync(id);
            Reviews = _review.getReviewsById(id);
            Services = _service.getServicesByMuaId(id);
            Portfolios = _portfo.getPortfolioItemsByMuaId(id);
            WorkingHours = _workingHours.GetWorkingHoursByMuaId(id);
            MuaProfile = _muaServices.GetMuaProfileById(id);
        }
        public async Task<IActionResult> OnPostAddReviewAsync(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                ModelState.AddModelError(string.Empty, "Bạn cần đăng nhập để đánh giá.");
                await OnGet(id);
                // Redirect to login page
                return RedirectToPage("/Auth/Login");
            }
            var booking = _bookingService.GetBookingById(userId.Value, id);

            if (booking == null)
            {
                ModelState.AddModelError(string.Empty, "Bạn cần có booking với MUA này trước khi đánh giá.");
                await OnGet(id);
                return Page();
            }
            ModelState.Remove("NewReview.Mua");
            ModelState.Remove("NewReview.Booking");
            ModelState.Remove("NewReview.Customer");
            ModelState.Remove("Mua");
            ModelState.Remove("Name");
            ModelState.Remove("Currency");
            // Gán thông tin còn thiếu
            NewReview.CustomerId = userId.Value; 
            NewReview.MuaId = id; 
            NewReview.BookingId = booking.BookingId; 
            NewReview.CreatedAt = DateTime.Now;
            if (!ModelState.IsValid)
            {
                await OnGet(id);
                return Page();
            }

            _review.addReview(NewReview);

            return RedirectToPage(new { id });
        }
    }
}

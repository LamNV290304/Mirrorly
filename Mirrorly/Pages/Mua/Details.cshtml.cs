using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Mirrorly.Models;
using Mirrorly.Services;
using Mirrorly.Services.Interfaces;
using System.Security.Claims;

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
        public List<Review> Reviews { get; set; }
        public List<Service> Services { get; set; }
        public List<PortfolioItem> Portfolios { get; set; }
        public List<WorkingHour> WorkingHours { get; set; }
        public Muaprofile MuaProfile { get; set; }
        [BindProperty]
        public Review NewReview { get; set; }
        public void OnGet(int id)
        {
            Reviews = _review.getReviewsById(id);
            Services = _service.getServicesByMuaId(id);
            Portfolios = _portfo.getPortfolioItemsByMuaId(id);
            WorkingHours = _workingHours.GetWorkingHoursByMuaId(id);
            MuaProfile = _muaServices.GetMuaProfileById(id);
        }
        public async Task<IActionResult> OnPostAsync(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                ModelState.AddModelError(string.Empty, "Bạn cần đăng nhập để đánh giá.");
                OnGet(id);
                // Redirect to login page
                return RedirectToPage("/Auth/Login");
            }
            // Lấy id user đang login
            // Kiểm tra booking tồn tại (ví dụ booking đã hoàn thành)
            var booking = _bookingService.GetBookingById(userId.Value, id);

            if (booking == null)
            {
                ModelState.AddModelError(string.Empty, "Bạn cần có booking với MUA này trước khi đánh giá.");
                OnGet(id);
                return Page();
            }

            // Kiểm tra NewReview null trước khi gán thuộc tính
            if (NewReview == null)
            {
                ModelState.AddModelError("", "Form gửi lên không hợp lệ.");
                OnGet(id);
                return Page();
            }

            // Gán thông tin còn thiếu
            NewReview.CustomerId = userId.Value;
            NewReview.MuaId = id;
            NewReview.BookingId = booking.BookingId;
            NewReview.CreatedAt = DateTime.Now;

            if (!ModelState.IsValid)
            {
                OnGet(id);
                return Page();
            }

            _review.addReview(NewReview);

            return RedirectToPage(new { id });
        }
    }
}

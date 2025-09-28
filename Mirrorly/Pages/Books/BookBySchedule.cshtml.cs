using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Mirrorly.Models;
using Mirrorly.Services.Interfaces;
using System.Globalization;

namespace Mirrorly.Pages.Books
{
    public class BookByScheduleModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string? Week { get; set; }  // kiểu "2025-W39"
        public long? Id { get; set; }
        public DateTime WeekStart { get; set; }
        public DateTime WeekEnd { get; set; }

        public List<BookingView> AcceptedBookings { get; set; } = new();

        private readonly IBookingService _bookingService;

        public BookByScheduleModel(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        public IActionResult OnGet(int id = 0)
        {
            
            var dbBookings = new List<Booking>();
            if (id == 0)
            {
                int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
                if (userId == 0)
                {
                    return RedirectToPage("/Auth/Login");
                }
                dbBookings = _bookingService.GetBookingsByMuaIdAndStatus(userId, 1);
                Id = userId;
            }
            else
            {
                dbBookings = _bookingService.GetBookingsByMuaIdAndStatus(id, 1);
            }
            if (string.IsNullOrEmpty(Week))
            {
                var today = DateTime.Today;
                var weekNum = ISOWeek.GetWeekOfYear(today);
                Week = $"{today.Year}-W{weekNum:D2}";
            }

            var parts = Week.Split("-W");
            int year = int.Parse(parts[0]);
            int weekNumInt = int.Parse(parts[1]);

            WeekStart = ISOWeek.ToDateTime(year, weekNumInt, DayOfWeek.Monday);
            WeekEnd = WeekStart.AddDays(6);

            AcceptedBookings = dbBookings.Select(b =>
            {
                var startHour = b.TimeM.HasValue ? b.TimeM.Value.Hours : 0;
                var startMinute = b.TimeM.HasValue ? b.TimeM.Value.Minutes : 0;
                var durationMin = b.Service.DurationMin;

                var start = new TimeSpan(startHour, startMinute, 0);
                var end = start.Add(TimeSpan.FromMinutes(durationMin));

                return new BookingView
                {
                    Title = $"Dịch vụ {b.Service.Name}",
                    Day = b.ScheduledStart.HasValue ? b.ScheduledStart.Value.DayOfWeek : DayOfWeek.Monday,
                    StartHour = start.Hours,
                    DurationHourCeil = (int)Math.Ceiling(durationMin / 60.0),
                    StartDisplay = start.ToString(@"hh\:mm"),
                    EndDisplay = end.ToString(@"hh\:mm")
                };
            }).ToList();


            return Page();
        }

        public class BookingView
        {
            public string Title { get; set; } = string.Empty;
            public DayOfWeek Day { get; set; }
            public int StartHour { get; set; }
            public int DurationHourCeil { get; set; }   // dùng cho rowspan (nguyên giờ)
            public string StartDisplay { get; set; } = string.Empty;
            public string EndDisplay { get; set; } = string.Empty;
        }

    }
}

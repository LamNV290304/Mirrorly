using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Mirrorly.Models;
using System.Globalization;

namespace Mirrorly.Pages.Books
{
    public class BookByScheduleModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string? Week { get; set; }  // kiểu "2025-W39"

        public DateTime WeekStart { get; set; }
        public DateTime WeekEnd { get; set; }

        public List<BookingView> AcceptedBookings { get; set; } = new();

        public void OnGet()
        {
            if (string.IsNullOrEmpty(Week))
            {
                var today = DateTime.Today;
                var weekNum = ISOWeek.GetWeekOfYear(today);
                Week = $"{today.Year}-W{weekNum:D2}";
            }

            // Parse Week (vd: "2025-W39")
            var parts = Week.Split("-W");
            int year = int.Parse(parts[0]);
            int weekNumInt = int.Parse(parts[1]);

            // Tính Monday của tuần đó
            WeekStart = ISOWeek.ToDateTime(year, weekNumInt, DayOfWeek.Monday);
            WeekEnd = WeekStart.AddDays(6);

            // Mock dữ liệu để test
            var dbBookings = new List<Booking>
{
    new Booking { BookingId = 1, CustomerId = 2, MuaId = 2, ScheduledStart = new DateTime(2025,9,24), TimeM = new TimeSpan(23,42,0), AddressLine="11", Status=3, ServiceId=1 },
    new Booking { BookingId = 2, CustomerId = 2, MuaId = 2, ScheduledStart = new DateTime(2025,9,24), TimeM = new TimeSpan(0,45,0), Status=3, ServiceId=1 },
    new Booking { BookingId = 3, CustomerId = 2, MuaId = 2, ScheduledStart = new DateTime(2025,9,26), TimeM = new TimeSpan(0,49,0), Status=3, ServiceId=1 },
    new Booking { BookingId = 4, CustomerId = 2, MuaId = 2, ScheduledStart = new DateTime(2025,9,27), TimeM = new TimeSpan(2,51,0), Status=3, ServiceId=1 },
    new Booking { BookingId = 5, CustomerId = 2, MuaId = 2, ScheduledStart = new DateTime(2025,9,30), TimeM = new TimeSpan(13,53,0), AddressLine="dhaoihdaodo", Status=3, ServiceId=1 }
};

            AcceptedBookings = dbBookings.Select(b => new BookingView
            {
                Title = $"Khách {b.CustomerId} - Dịch vụ {b.ServiceId}",
                Day = b.ScheduledStart.HasValue ? b.ScheduledStart.Value.DayOfWeek : DayOfWeek.Monday,
                StartHour = b.TimeM.HasValue ? b.TimeM.Value.Hours : 0,
                Duration = 1                  // tạm fix 1 giờ, bạn có thể thêm cột Duration vào DB nếu muốn chính xác
            }).ToList();

        }

        public class BookingView
        {
            public string Title { get; set; } = string.Empty;
            public DayOfWeek Day { get; set; }
            public int StartHour { get; set; }
            public int Duration { get; set; }
        }
    }
}

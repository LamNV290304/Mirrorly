//using Microsoft.AspNetCore.Mvc.RazorPages;
//using System.Collections.Generic;

//namespace Mirrorly.Pages.Books
//{
//    public class BookingStatusModel : PageModel
//    {
//        public List<BookingInfo> Bookings { get; set; }

//        public void OnGet()
//        {
//            // Demo dữ liệu giả (sau này bạn có thể lấy từ DB)
//            Bookings = new List<BookingInfo>
//            {
//                new BookingInfo { Name = "Nguyễn An", Service = "Makeup Dự Tiệc", Date = "2025-09-21", Time = "18:00", Status = 1 },
//                new BookingInfo { Name = "Trần Bình", Service = "Makeup Cô Dâu", Date = "2025-09-23", Time = "09:00", Status = 2 },
//                new BookingInfo { Name = "Lê Cẩm", Service = "Makeup Cơ Bản", Date = "2025-09-25", Time = "14:30", Status = 3 }
//            };
//        }
//    }

//    public class BookingInfo
//    {
//        public string Name { get; set; }
//        public string Service { get; set; }
//        public string Date { get; set; }
//        public string Time { get; set; }
//        public int Status { get; set; }
//    }
//}

//using Microsoft.AspNetCore.Mvc.RazorPages;
//using System.Collections.Generic;

//namespace Mirrorly.Pages.Orders
//{
//    public class ArtistOrdersModel : PageModel
//    {
//        public List<OrderInfo> Orders { get; set; }

//        public void OnGet()
//        {
//            Orders = new List<OrderInfo>
//            {
//                new OrderInfo {
//                    CustomerName = "Nguyễn An",
//                    Phone = "0912345678",
//                    Service = "Makeup Cô Dâu",
//                    Date = "2025-09-30",
//                    Time = "09:00",
//                    Address = "123 Đường Hoa Hồng, Hà Nội",
//                    Notes = "Makeup tự nhiên",
//                    Status = "2" // Confirmed
//                },
//                new OrderInfo {
//                    CustomerName = "Trần Bình",
//                    Phone = "0987654321",
//                    Service = "Makeup Dự Tiệc",
//                    Date = "2025-10-01",
//                    Time = "18:30",
//                    Address = "456 Đường Lê Lợi, HCM",
//                    Notes = "Da nhạy cảm",
//                    Status = "2" // Pending
//                },
//                new OrderInfo {
//                    CustomerName = "Lê Cẩm",
//                    Phone = "0909090909",
//                    Service = "Makeup Cơ Bản",
//                    Date = "2025-10-05",
//                    Time = "14:00",
//                    Address = "789 Đường Trần Hưng Đạo, Đà Nẵng",
//                    Notes = "",
//                    Status = "2" // Cancelled
//                }
//            };

//            foreach (var o in Orders)
//            {
//                switch (o.Status)
//                {
//                    case "1":
//                        o.StatusClass = "confirmed";
//                        o.StatusText = "CONFIRMED";
//                        break;
//                    case "2":
//                        o.StatusClass = "pending";
//                        o.StatusText = "PENDING";
//                        break;
//                    case "3":
//                        o.StatusClass = "cancelled";
//                        o.StatusText = "CANCELLED";
//                        break;
//                }
//            }
//        }
//    }

//    public class OrderInfo
//    {
//        public string CustomerName { get; set; }
//        public string Phone { get; set; }
//        public string Service { get; set; }
//        public string Date { get; set; }
//        public string Time { get; set; }
//        public string Address { get; set; }
//        public string Notes { get; set; }
//        public string Status { get; set; }
//        public string StatusClass { get; set; }
//        public string StatusText { get; set; }
//    }
//}

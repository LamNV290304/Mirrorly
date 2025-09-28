using Microsoft.EntityFrameworkCore;
using Mirrorly.Models;
using Mirrorly.Repositories.Interfaces;
using Mirrorly.Services.Interfaces;

namespace Mirrorly.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepo _bookingRepo;
        private readonly ProjectExeContext _context = new ProjectExeContext();

        public BookingService(IBookingRepo bookingRepo)
        {
            _bookingRepo = bookingRepo;
        }
        public void AddBooking(Booking booking)
        {
            _bookingRepo.AddBooking(booking);
        }

        public void ChangeBookingStatus(int bookingId, int status)
        {
            _bookingRepo.ChangeBookingStatus(bookingId, status);
        }

        public Booking GetBookingById(int cusId, int muaId)
        {
            return _bookingRepo.GetBookingById(cusId, muaId);
        }

        public List<Booking> GetBookingsByCustomerId(int cusId)
        {
            return _bookingRepo.GetBookingsByCustomerId(cusId);
        }

        public List<Booking> GetBookingsByMuaId(int muaId)
        {
            return _bookingRepo.GetBookingsByMuaId(muaId);
        }

        public List<Booking> GetBookingsByMuaIdAndStatus(int muaId, int status)
        {
            return _bookingRepo.GetBookingsByMuaIdAndStatus(muaId, status);
        }

        public BookingDetailViewModel? GetBookingDetail(long bookingId)
        {
            var booking = _context.Bookings
                .Include(b => b.Service)
                .FirstOrDefault(b => b.BookingId == bookingId);
            if (booking == null) return null;

            var customerProfile = _context.CustomerProfiles
                .FirstOrDefault(c => c.CustomerId == booking.CustomerId);

            var user = _context.Users
                .FirstOrDefault(u => u.UserId == booking.CustomerId);

            return new BookingDetailViewModel
            {
                BookingId = booking.BookingId,
                ScheduledStart = booking.ScheduledStart,
                AddressLine = booking.AddressLine,
                Notes = booking.Notes,
                TimeM = booking.TimeM,
                ServiceName = booking.Service.Name, // nếu có bảng Services thì join lấy tên

                CustomerId = booking.CustomerId,
                DisplayName = customerProfile?.DisplayName,
                AvatarUrl = customerProfile?.AvatarUrl,
                PhoneNumber = customerProfile?.PhoneNumber,

                Email = user?.Email,
                Username = user?.Username
            };
        }

        public Booking GetBookingByBookingId(int bookingId)
        {
            return _bookingRepo.GetBookingByBookId(bookingId);
        }
    }
}

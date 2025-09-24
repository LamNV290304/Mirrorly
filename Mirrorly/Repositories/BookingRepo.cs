using Microsoft.EntityFrameworkCore;
using Mirrorly.Models;
using Mirrorly.Repositories.Interfaces;

namespace Mirrorly.Repositories
{
    public class BookingRepo : IBookingRepo
    {
        private readonly ProjectExeContext _context;
        public BookingRepo(ProjectExeContext context)
        {
            _context = context;
        }
        public void AddBooking(Booking booking)
        {
            _context.Bookings.Add(booking);
            _context.SaveChanges();
        }

        public void ChangeBookingStatus(int bookingId, int status)
        {
            var booking = _context.Bookings.Find(bookingId);
            if (booking != null)
            {
                booking.Status = status;
                _context.SaveChanges();
            }
        }

        public Booking GetBookingById(int cusId, int muaId)
        {
           return _context.Bookings.FirstOrDefault(b => b.CustomerId == cusId && b.MuaId == muaId) ;
        }

        public List<Booking> GetBookingsByCustomerId(int cusId)
        {
            return _context.Bookings.Where(b => b.CustomerId == cusId).ToList();
        }

        public List<Booking> GetBookingsByMuaId(int muaId)
        {
            return _context.Bookings.Where(b => b.MuaId == muaId).ToList();
        }

        public List<Booking> GetBookingsByMuaIdAndStatus(int muaId, int status)
        {
            return _context.Bookings.Where(b => b.MuaId == muaId && b.Status == status).ToList();
        }
    }
}

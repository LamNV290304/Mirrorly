using Mirrorly.Models;
using Mirrorly.Repositories.Interfaces;
using Mirrorly.Services.Interfaces;

namespace Mirrorly.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepo _bookingRepo;

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
    }
}

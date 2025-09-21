using Mirrorly.Models;

namespace Mirrorly.Services.Interfaces
{
    public interface IBookingService
    {
        void AddBooking(Booking booking);

        List<Booking> GetBookingsByCustomerId(int cusId);

        void ChangeBookingStatus(int bookingId, int status);

        List<Booking> GetBookingsByMuaId(int muaId);

        List<Booking> GetBookingsByMuaIdAndStatus(int muaId, int status);
    }
}

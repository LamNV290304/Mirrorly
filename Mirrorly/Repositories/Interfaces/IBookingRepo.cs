using Mirrorly.Models;

namespace Mirrorly.Repositories.Interfaces
{
    public interface IBookingRepo
    {
        void AddBooking(Booking booking);
        Booking GetBookingById(int cusId, int muaId);

        List<Booking> GetBookingsByCustomerId(int cusId);

        void ChangeBookingStatus(int bookingId, int status);

        List<Booking> GetBookingsByMuaId(int muaId);
        List<Booking> GetBookingsByMuaIdAndStatus(int muaId, int status);
    }
}

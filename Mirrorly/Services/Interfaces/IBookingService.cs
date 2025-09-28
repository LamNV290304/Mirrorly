using Mirrorly.Models;

namespace Mirrorly.Services.Interfaces
{
    public interface IBookingService
    {
        void AddBooking(Booking booking);
        Booking GetBookingById(int cusId, int muaId);
        Booking GetBookingByBookingId(int bookingId);

        List<Booking> GetBookingsByCustomerId(int cusId);

        void ChangeBookingStatus(int bookingId, int status);

        List<Booking> GetBookingsByMuaId(int muaId);

        List<Booking> GetBookingsByMuaIdAndStatus(int muaId, int status);
        BookingDetailViewModel? GetBookingDetail(long bookingId);
    }

    public class BookingDetailViewModel
    {
        // Booking
        public long BookingId { get; set; }
        public DateTime? ScheduledStart { get; set; }
        public string? AddressLine { get; set; }
        public string? Notes { get; set; }
        public TimeSpan? TimeM { get; set; }
        public string? ServiceName { get; set; }

        // Customer profile
        public int CustomerId { get; set; }
        public string? DisplayName { get; set; }
        public string? AvatarUrl { get; set; }
        public string? PhoneNumber { get; set; }

        // User info
        public string? Email { get; set; }
        public string? Username { get; set; }
    }
}

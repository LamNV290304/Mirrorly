using System;
using System.Collections.Generic;

namespace Mirrorly.Models;

public partial class Booking
{
    public long BookingId { get; set; }

    public int CustomerId { get; set; }

    public int MuaId { get; set; }

    public DateTime? ScheduledStart { get; set; }

    public DateTime? ScheduledEnd { get; set; }

    public string? AddressLine { get; set; }

    public string Currency { get; set; } = null!;

    public string? Notes { get; set; }

    public int? Status { get; set; }

    public virtual ICollection<BookingItem> BookingItems { get; set; } = new List<BookingItem>();

    public virtual CustomerProfile Customer { get; set; } = null!;

    public virtual Muaprofile Mua { get; set; } = null!;

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}

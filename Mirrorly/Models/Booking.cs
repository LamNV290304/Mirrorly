using System;
using System.Collections.Generic;

namespace Mirrorly.Models;

public partial class Booking
{
    public long BookingId { get; set; }

    public int CustomerId { get; set; }

    public int MuaId { get; set; }

    public DateTime? ScheduledStart { get; set; }

    public string? AddressLine { get; set; }

    public string? Notes { get; set; }

    public int Status { get; set; }

    public TimeSpan? TimeM { get; set; }

    public long? ServiceId { get; set; }

    public virtual CustomerProfile Customer { get; set; } = null!;

    public virtual Muaprofile Mua { get; set; } = null!;

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual Service? Service { get; set; }
}

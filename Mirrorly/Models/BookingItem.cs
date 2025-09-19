using System;
using System.Collections.Generic;

namespace Mirrorly.Models;

public partial class BookingItem
{
    public long BookingItemId { get; set; }

    public long BookingId { get; set; }

    public long ServiceId { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public virtual Booking Booking { get; set; } = null!;

    public virtual Service Service { get; set; } = null!;
}

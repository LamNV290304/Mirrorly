using System;
using System.Collections.Generic;

namespace Mirrorly.Models;

public partial class Review
{
    public long ReviewId { get; set; }

    public long BookingId { get; set; }

    public int CustomerId { get; set; }

    public int MuaId { get; set; }

    public byte Rating { get; set; }

    public string? Comment { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? Attachment { get; set; }

    public virtual Booking Booking { get; set; } = null!;

    public virtual CustomerProfile Customer { get; set; } = null!;

    public virtual Muaprofile Mua { get; set; } = null!;
}

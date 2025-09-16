using System;
using System.Collections.Generic;

namespace Mirrorly.Models;

public partial class CustomerProfile
{
    public int CustomerId { get; set; }

    public string? DisplayName { get; set; }

    public string? AvatarUrl { get; set; }

    public string? PhoneNumber { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual User Customer { get; set; } = null!;

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}

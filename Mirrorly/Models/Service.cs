using System;
using System.Collections.Generic;

namespace Mirrorly.Models;

public partial class Service
{
    public long ServiceId { get; set; }

    public int MuaId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public decimal BasePrice { get; set; }

    public string Currency { get; set; } = null!;

    public int DurationMin { get; set; }

    public bool Active { get; set; }

    public int? CategoryId { get; set; }

    public virtual ICollection<BookingItem> BookingItems { get; set; } = new List<BookingItem>();

    public virtual Category? Category { get; set; }

    public virtual Muaprofile Mua { get; set; } = null!;
}

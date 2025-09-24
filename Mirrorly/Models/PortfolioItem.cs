using System;
using System.Collections.Generic;

namespace Mirrorly.Models;

public partial class PortfolioItem
{
    public long ItemId { get; set; }

    public int Muaid { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public string? MediaUrl { get; set; } = null!;

    public DateTime CreatedAtUtc { get; set; }

    public virtual Muaprofile? Mua { get; set; } = null!;
}

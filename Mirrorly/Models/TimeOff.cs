using System;
using System.Collections.Generic;

namespace Mirrorly.Models;

public partial class TimeOff
{
    public long TimeOffId { get; set; }

    public int MuaId { get; set; }

    public DateTime StartUtc { get; set; }

    public DateTime EndUtc { get; set; }

    public string? Reason { get; set; }

    public virtual Muaprofile Mua { get; set; } = null!;
}

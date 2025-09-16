using System;
using System.Collections.Generic;

namespace Mirrorly.Models;

public partial class WorkingHour
{
    public long WorkingHourId { get; set; }

    public int MuaId { get; set; }

    public byte DayOfWeek { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public virtual Muaprofile Mua { get; set; } = null!;
}

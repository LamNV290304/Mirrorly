using System;
using System.Collections.Generic;

namespace Mirrorly.Models;

public partial class SlotHaveBook
{
    public int Id { get; set; }

    public int? SlotId { get; set; }

    public DateTime? ScheduleDate { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual Slot? Slot { get; set; }
}

using System;
using System.Collections.Generic;

namespace Mirrorly.Models;

public partial class Slot
{
    public int Id { get; set; }

    public DateTime? TimeStart { get; set; }

    public DateTime? TimeEnd { get; set; }

    public virtual ICollection<SlotHaveBook> SlotHaveBooks { get; set; } = new List<SlotHaveBook>();
}

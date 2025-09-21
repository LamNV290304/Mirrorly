using System;
using System.Collections.Generic;

namespace Mirrorly.Models;

public partial class Muaprofile
{
    public int Muaid { get; set; }

    public string DisplayName { get; set; } = null!;

    public string? Bio { get; set; }

    public string? AddressLine { get; set; }

    public int? ExperienceYears { get; set; }

    public bool ProfilePublic { get; set; }
    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual User Mua { get; set; } = null!;

    public virtual ICollection<PortfolioItem> PortfolioItems { get; set; } = new List<PortfolioItem>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ICollection<Service> Services { get; set; } = new List<Service>();

    public virtual ICollection<TimeOff> TimeOffs { get; set; } = new List<TimeOff>();

    public virtual ICollection<WorkingHour> WorkingHours { get; set; } = new List<WorkingHour>();
}

using System;
using System.Collections.Generic;

namespace Mirrorly.Models;

public partial class Category
{
    public int CategoryId { get; set; }

    public string CategoryName { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<Service> Services { get; set; } = new List<Service>();
}

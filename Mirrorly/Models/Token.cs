using System;
using System.Collections.Generic;

namespace Mirrorly.Models;

public partial class Token
{
    public string Email { get; set; } = null!;

    public string Token1 { get; set; } = null!;

    public DateTime Expired { get; set; }

    public bool Status { get; set; }
}

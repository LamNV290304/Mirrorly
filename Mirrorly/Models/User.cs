using System;
using System.Collections.Generic;

namespace Mirrorly.Models;

public partial class User
{
    public int UserId { get; set; }

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public byte[] PasswordHash { get; set; } = null!;

    public string? FullName { get; set; }

    public string? PhoneNumber { get; set; }

    public string? AvatarUrl { get; set; }

    public bool IsActive { get; set; }

    public bool IsEmailVerified { get; set; }

    public byte RoleId { get; set; }

    public virtual CustomerProfile? CustomerProfile { get; set; }

    public virtual Muaprofile? Muaprofile { get; set; }

    public virtual Role Role { get; set; } = null!;
}

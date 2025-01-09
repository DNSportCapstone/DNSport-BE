using System;
using System.Collections.Generic;

namespace BusinessObject.Models;

public partial class UserDetail
{
    public int UserId { get; set; }

    public string? PhoneNumber { get; set; }

    public string? FullName { get; set; }

    public string? Address { get; set; }

    public virtual User User { get; set; } = null!;
}

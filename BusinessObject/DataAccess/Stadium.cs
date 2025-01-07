using System;
using System.Collections.Generic;

namespace BusinessObject.DataAccess;

public partial class Stadium
{
    public int StadiumId { get; set; }

    public int? UserId { get; set; }

    public string? StadiumName { get; set; }

    public string? Address { get; set; }

    public string? Image { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<Field> Fields { get; set; } = new List<Field>();

    public virtual User? User { get; set; }
}

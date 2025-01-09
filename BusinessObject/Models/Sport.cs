using System;
using System.Collections.Generic;

namespace BusinessObject.Models;

public partial class Sport
{
    public int SportId { get; set; }

    public string? SportName { get; set; }

    public virtual ICollection<Field> Fields { get; set; } = new List<Field>();
}

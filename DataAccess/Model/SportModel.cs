using System;
using System.Collections.Generic;

namespace DataAccess.Model;

public partial class SportModel
{
    public int SportId { get; set; }

    public string? SportName { get; set; }

    public virtual ICollection<FieldModel> Fields { get; set; } = new List<FieldModel>();
}

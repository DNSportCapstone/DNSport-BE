using System;
using System.Collections.Generic;

namespace BusinessObject.DataAccess;

public partial class ServiceCategory
{
    public int CategoryId { get; set; }

    public string? CategoryName { get; set; }

    public virtual ICollection<Service> Services { get; set; } = new List<Service>();
}

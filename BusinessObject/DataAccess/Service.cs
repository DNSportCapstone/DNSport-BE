using System;
using System.Collections.Generic;

namespace BusinessObject.DataAccess;

public partial class Service
{
    public int ServiceId { get; set; }

    public int? FieldId { get; set; }

    public int? CategoryId { get; set; }

    public string? ServiceName { get; set; }

    public decimal? Price { get; set; }

    public string? Description { get; set; }

    public int? Inventory { get; set; }

    public virtual ICollection<BookingFieldService> BookingFieldServices { get; set; } = new List<BookingFieldService>();

    public virtual ServiceCategory? Category { get; set; }

    public virtual Field? Field { get; set; }
}

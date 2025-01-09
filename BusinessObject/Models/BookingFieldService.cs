using System;
using System.Collections.Generic;

namespace BusinessObject.Models;

public partial class BookingFieldService
{
    public int BookingFieldId { get; set; }

    public int ServiceId { get; set; }

    public decimal? TotalPrice { get; set; }

    public int? Quantity { get; set; }

    public decimal? Price { get; set; }

    public virtual BookingField BookingField { get; set; } = null!;

    public virtual Service Service { get; set; } = null!;
}

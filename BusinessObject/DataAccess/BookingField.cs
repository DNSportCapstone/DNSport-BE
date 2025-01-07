using System;
using System.Collections.Generic;

namespace BusinessObject.DataAccess;

public partial class BookingField
{
    public int BookingFieldId { get; set; }

    public int? BookingId { get; set; }

    public int? FieldId { get; set; }

    public DateTime? StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public decimal? Price { get; set; }

    public DateTime? Date { get; set; }

    public virtual Booking? Booking { get; set; }

    public virtual ICollection<BookingFieldService> BookingFieldServices { get; set; } = new List<BookingFieldService>();

    public virtual Field? Field { get; set; }
}

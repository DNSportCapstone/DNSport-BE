using System;
using System.Collections.Generic;

namespace BusinessObject.DataAccess;

public partial class Rating
{
    public int RatingId { get; set; }

    public int? UserId { get; set; }

    public int? BookingId { get; set; }

    public int? RatingValue { get; set; }

    public string? Comment { get; set; }

    public DateTime? Time { get; set; }

    public string? Reply { get; set; }

    public DateTime? ReplyTime { get; set; }

    public virtual Booking? Booking { get; set; }

    public virtual User? User { get; set; }
}

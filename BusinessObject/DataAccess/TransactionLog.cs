using System;
using System.Collections.Generic;

namespace BusinessObject.DataAccess;

public partial class TransactionLog
{
    public int LogId { get; set; }

    public int? UserId { get; set; }

    public int? BookingId { get; set; }

    public DateTime? TimeSlot { get; set; }

    public string? TransactionType { get; set; }

    public string? ErrorMessage { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Booking? Booking { get; set; }

    public virtual User? User { get; set; }
}

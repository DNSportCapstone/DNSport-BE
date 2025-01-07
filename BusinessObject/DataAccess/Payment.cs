using System;
using System.Collections.Generic;

namespace BusinessObject.DataAccess;

public partial class Payment
{
    public int PaymentId { get; set; }

    public int? BookingId { get; set; }

    public decimal? Deposit { get; set; }

    public DateTime? PaymentTime { get; set; }

    public string? Status { get; set; }

    public virtual Booking? Booking { get; set; }

    public virtual ICollection<Refund> Refunds { get; set; } = new List<Refund>();
}

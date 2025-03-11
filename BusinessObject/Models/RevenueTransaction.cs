using System;
using System.Collections.Generic;

namespace BusinessObject.Models;

public partial class RevenueTransaction
{
    public int RevenueTransactionId { get; set; }

    public int? BookingId { get; set; }

    public decimal? TotalRevenue { get; set; }

    public decimal? AdminAmount { get; set; }

    public decimal? OwnerAmount { get; set; }

    public DateTime? RevenueTransactionDate { get; set; }

    public string? Status { get; set; }

    public virtual Booking? Booking { get; set; }
}

using System;
using System.Collections.Generic;

namespace BusinessObject.Models;

public partial class Booking
{
    public int BookingId { get; set; }

    public int? UserId { get; set; }

    public int? VoucherId { get; set; }

    public decimal? TotalPrice { get; set; }

    public DateTime? BookingDate { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<BookingField> BookingFields { get; set; } = new List<BookingField>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();

    public virtual ICollection<RevenueTransaction> RevenueTransactions { get; set; } = new List<RevenueTransaction>();

    public virtual ICollection<TransactionLog> TransactionLogs { get; set; } = new List<TransactionLog>();

    public virtual User? User { get; set; }

    public virtual Voucher? Voucher { get; set; }
}

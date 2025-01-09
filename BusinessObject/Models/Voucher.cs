using System;
using System.Collections.Generic;

namespace BusinessObject.Models;

public partial class Voucher
{
    public int VoucherId { get; set; }

    public string? VoucherCode { get; set; }

    public decimal? DiscountPercentage { get; set; }

    public DateTime? ExpiryDate { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<UserVoucher> UserVouchers { get; set; } = new List<UserVoucher>();
}

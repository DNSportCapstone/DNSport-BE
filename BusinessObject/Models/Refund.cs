﻿using System;
using System.Collections.Generic;

namespace BusinessObject.Models;

public partial class Refund
{
    public int RefundId { get; set; }

    public int? UserId { get; set; }

    public int? PaymentId { get; set; }

    public decimal? RefundAmount { get; set; }

    public string? Status { get; set; }

    public DateTime? Time { get; set; }

    public virtual Payment? Payment { get; set; }

    public virtual User? User { get; set; }
}

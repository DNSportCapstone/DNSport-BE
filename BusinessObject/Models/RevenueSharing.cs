using System;
using System.Collections.Generic;

namespace BusinessObject.Models;

public partial class RevenueSharing
{
    public int RevenueSharingId { get; set; }

    public int? StadiumId { get; set; }

    public decimal? LessorPercentage { get; set; }

    public string? Status { get; set; }

    public virtual Stadium? Stadium { get; set; }
}

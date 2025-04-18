using System;
using System.Collections.Generic;

namespace BusinessObject.Models;

public partial class Denounce
{
    public int DenounceId { get; set; }

    public int? SendId { get; set; }

    public int? ReceiveId { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public DateTime? DenounceTime { get; set; }

    public int BookingId { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<DenounceImage> DenounceImages { get; set; } = new List<DenounceImage>();

    public virtual User? Receive { get; set; }

    public virtual User? Send { get; set; }
}

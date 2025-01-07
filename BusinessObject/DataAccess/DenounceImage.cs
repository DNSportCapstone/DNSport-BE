using System;
using System.Collections.Generic;

namespace BusinessObject.DataAccess;

public partial class DenounceImage
{
    public int DenounceImageId { get; set; }

    public int? DenounceId { get; set; }

    public int? PublicId { get; set; }

    public string? Url { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Denounce? Denounce { get; set; }
}

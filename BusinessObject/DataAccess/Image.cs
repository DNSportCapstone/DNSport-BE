using System;
using System.Collections.Generic;

namespace BusinessObject.DataAccess;

public partial class Image
{
    public int ImageId { get; set; }

    public int? FieldId { get; set; }

    public int? PublicId { get; set; }

    public string? Url { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Field? Field { get; set; }
}

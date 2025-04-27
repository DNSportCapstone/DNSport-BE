using System;
using System.Collections.Generic;

namespace BusinessObject.Models;

public partial class Field
{
    public int FieldId { get; set; }

    public int? StadiumId { get; set; }

    public int? SportId { get; set; }

    public string? Description { get; set; }

    public decimal? DayPrice { get; set; }

    public decimal? NightPrice { get; set; }

    public string? Status { get; set; }

    public string? FieldName { get; set; }

    public int? MaximumPeople { get; set; }


    public virtual ICollection<BookingField> BookingFields { get; set; } = new List<BookingField>();

    public virtual ICollection<Image> Images { get; set; } = new List<Image>();

    public virtual ICollection<Service> Services { get; set; } = new List<Service>();

    public virtual Sport? Sport { get; set; }

    public virtual Stadium? Stadium { get; set; }
}

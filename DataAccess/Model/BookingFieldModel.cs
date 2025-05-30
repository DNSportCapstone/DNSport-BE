﻿namespace DataAccess.Model
{
    public class BookingFieldModel
    {
        public int BookingFieldId { get; set; }
        public int? BookingId { get; set; }
        public int? FieldId { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public decimal? Price { get; set; }
        public DateTime? Date { get; set; }
        public virtual FieldModel? Field { get; set; }
    }
}

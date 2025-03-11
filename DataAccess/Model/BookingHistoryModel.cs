namespace DataAccess.Model
{
    public class BookingHistoryModel
    {
        public int BookingId { get; set; }
        public int? VoucherId { get; set; }
        public decimal? TotalPrice { get; set; }
        public DateTime? BookingDate { get; set; }
        public string? Status { get; set; }
        public int? FieldId { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string? StadiumName { get; set; }
    }
}

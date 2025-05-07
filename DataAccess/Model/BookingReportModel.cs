namespace DataAccess.Model
{
    public class BookingReportModel
    {
        public int? UserId { get; set; }
        public string? UserName { get; set; }
        public DateTime? BookingTime { get; set; }
        public string? Type { get; set; }
        public int? SportId { get; set; }
    }
}

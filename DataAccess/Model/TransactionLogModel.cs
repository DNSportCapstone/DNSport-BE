namespace DataAccess.Model
{
    public class TransactionLogModel
    {
        public int LogId { get; set; }

        public int? UserId { get; set; }

        public int? BookingId { get; set; }

        public DateTime? TimeSlot { get; set; }

        public string? TransactionType { get; set; }

        public string? ErrorMessage { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
        public string? Amount { get; set; }
    }
}

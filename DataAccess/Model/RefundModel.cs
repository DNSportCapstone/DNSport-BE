namespace DataAccess.Model
{
    public class RefundModel
    {
        public int RefundId { get; set; }

        public int UserId { get; set; }

        public int PaymentId { get; set; }

        public decimal RefundAmount { get; set; }

        public string Status { get; set; } 

        public DateTime Time { get; set; }

        public string UserName { get; set; } 
        public string BankAccountNumber { get; set; } 
    }
}

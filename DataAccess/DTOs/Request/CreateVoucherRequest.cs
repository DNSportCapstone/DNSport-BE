namespace DataAccess.DTOs.Request
{
    public class CreateVoucherRequest
    {
        public int? VoucherId { get; set; }
        public string VoucherCode { get; set; }
        public int DiscountPercentage { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}

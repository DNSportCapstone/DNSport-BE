using System;

namespace DataAccess.DTOs.Request
{
    public class CreateVoucherRequest
    {
        public string VoucherCode { get; set; }
        public double DiscountPercentage { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
        
    public class CreateOrUpdateVoucherRequest
    {
        public int? VoucherId { get; set; }
        public string VoucherCode { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}

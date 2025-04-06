using System;

namespace DataAccess.DTOs.Request
{
    public class CreateVoucherRequest
    {
        public string VoucherCode { get; set; }
        public double DiscountPercentage { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string Conditions { get; set; }
    }
}

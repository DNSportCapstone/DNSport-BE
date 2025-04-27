using System;

namespace DataAccess.Model
{
    public class VoucherModel
    {
        public int VoucherId { get; set; }
        public string VoucherCode { get; set; }
        public double DiscountPercentage { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string Conditions { get; set; }
    }
}

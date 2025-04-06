using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DTOs.Response
{
    public class VoucherResponse
    {
        public int VoucherId { get; set; }
        public string VoucherCode { get; set; }
        public double DiscountPercentage { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string Conditions { get; set; }
        public string Status { get; set; }
    }
}

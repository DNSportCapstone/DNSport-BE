using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DTOs.Request
{
    public class UpdateVoucherRequest
    {
        public string VoucherCode { get; set; }
        public double DiscountPercentage { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}

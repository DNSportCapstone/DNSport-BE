using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Model
{
    public class RefundRequestModel
    {
        public int BookingId { get; set; }
        public string UserName { get; set; }
        public string Bank { get; set; }
        public string BankAccountNumber { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DTOs.Response
{
    public class RefundResponseModel : Response
    {
        public int RefundId { get; set; }
        public decimal RefundAmount { get; set; }
        public string TimeRemaining { get; set; }
        public double TimeRemainingInSeconds { get; set; }
        public decimal RefundPercentage { get; set; }
        public string Bank { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs.Request
{
    public class AproveOrRejectStadiumRequest
    {
        public int StadiumId { get; set; }
        public string Status { get; set; } // Expected values: "Active" or "Rejected"
    }
}
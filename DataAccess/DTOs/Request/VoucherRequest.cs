using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DTOs.Request
{
    public class VoucherRequest
    {
        public int UserId { get; set; }
        public string VoucherCode { get; set; } = string.Empty;
    }
}

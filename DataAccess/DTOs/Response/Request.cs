using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DTOs.Response
{
    public class Request
    {
        public bool? IsError { get; set; }
        public string? Message { get; set; }
        public string? Error { get; set; }
    }
    public class RefundResponseModel : Request
    {
        public string? Id { get; set; }
    }
}

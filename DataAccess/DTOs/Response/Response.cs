using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DTOs.Response
{
    public class Response
    {
        public bool? IsError { get; set; }
        public string? Message { get; set; }
        public string? Error { get; set; }
    }
}

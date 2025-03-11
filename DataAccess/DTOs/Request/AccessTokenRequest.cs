using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DTOs.Request
{
    public class AccessTokenRequest
    {
        public string? RefreshToken { get; set; }
    }
}

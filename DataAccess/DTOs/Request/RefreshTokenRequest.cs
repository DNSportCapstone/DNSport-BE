using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DTOs.Request
{
    public class RefreshTokenRequest
    {
        public string? AccessToken { get; set; }
    }
}

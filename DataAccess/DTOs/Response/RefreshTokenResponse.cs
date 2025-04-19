using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DTOs.Response
{
    public class RefreshTokenResponse : Response
    {
        public string? RefreshToken { get; set; }
    }
}

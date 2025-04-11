using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DTOs.Response
{
    public class GoogleLoginResponse : Response
    {
        public string? AccessToken { get; set; }
    }
}

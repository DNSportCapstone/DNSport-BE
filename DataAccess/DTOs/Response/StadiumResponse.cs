using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DTOs.Response
{
    public class StadiumResponse
    {
        public int StadiumId { get; set; }
        public string StadiumName { get; set; }
        public string Address { get; set; }
        public string Status { get; set; }
        public string Image { get; set; }
    }
}
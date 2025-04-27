using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Model
{
    public class StadiumLessorModel
    {
        public int StadiumId { get; set; }
        public string StadiumName { get; set; }
        public string Address { get; set; }
        public string Status { get; set; }
        public string LessorName { get; set; }
        public string Email { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Model
{
    public class StadiumDistanceModel
    {
        public int StadiumId { get; set; }
        public string StadiumName { get; set; }
        public string Address { get; set; }
        public string Image { get; set; }  
        public string Distance { get; set; }
        public string Duration  { get; set; } 
        public double DistanceValue { get; set; }
    }

}

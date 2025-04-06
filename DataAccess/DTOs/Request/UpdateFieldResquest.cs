using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DTOs.Request
{
    public class EditFieldRequest
    {
        public int FieldId { get; set; }
        public int StadiumId { get; set; }
        public int SportId { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal? DayPrice { get; set; } 
        public decimal? NightPrice { get; set; } 
        public string? Status { get; set; }
        public List<string> ImageUrls { get; set; }
    }

}

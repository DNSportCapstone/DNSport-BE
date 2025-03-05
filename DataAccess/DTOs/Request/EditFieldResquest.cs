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
        public string Description { get; set; } = string.Empty;
        public decimal? DayPrice { get; set; }  // Cho phép null
        public decimal? NightPrice { get; set; } // Cho phép null
        public string? Status { get; set; }
        //public IFormFile? Image { get; set; }
    }

}

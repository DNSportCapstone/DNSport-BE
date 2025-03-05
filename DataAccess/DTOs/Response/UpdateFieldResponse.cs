using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DTOs.Response
{
    public class UpdateFieldResponse
    {
        public int FieldId { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal DayPrice { get; set; }
        public decimal NightPrice { get; set; }
        public string Status { get; set; } = "pending";
        public string ImageUrl { get; set; } = string.Empty;
        public string Message { get; set; } = "Field updated successfully.";
    }

}

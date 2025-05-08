using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DTOs.Request
{
    public class RegisterFieldRequest
    {
        public int StadiumId { get; set; }
        public int SportId { get; set; }
        public string? FieldName { get; set; }
        public string? Description { get; set; }
        public decimal DayPrice { get; set; }
        public decimal NightPrice { get; set; }
        public int MaximumPeople { get; set; }
        public string Status { get; set; }
        public List<string> ImageUrls { get; set; }
    }
}

